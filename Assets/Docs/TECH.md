# Hit Tower 技术文档

## 项目概述

本项目是一个**中等体量的轻量塔防游戏**，采用 2.5D/俯视视角，核心特色为**数据驱动的 Buff 系统**和**灵活的波次配置**。项目使用 Unity 引擎开发，采用 C# 脚本，遵循规范的命名与架构设计。

---

## 1. 系统架构

### 1.1 整体架构

项目采用**分层架构 + 管理器模式**，主要分为以下层次：

```
┌─────────────────────────────────────┐
│         GameManager (单例)          │  ← 游戏生命周期管理
├─────────────────────────────────────┤
│  Manager 层（子系统管理）            │
│  ├── BuildingManager                │  ← 塔管理、范围检测
│  ├── EnemyManager                   │  ← 敌人管理、生成
│  ├── PlayerManager                  │  ← 玩家管理、经济
│  ├── BuffManager                    │  ← Buff管理、商店Buff
│  ├── ShopManager                    │  ← 商店管理
│  └── ArchiveManager                 │  ← 存档管理
├─────────────────────────────────────┤
│  Logic 层（业务逻辑）                │
│  ├── BuildingLogic                  │  ← 塔逻辑、升级
│  ├── EnemyLogic                     │  ← 敌人逻辑、伤害计算
│  └── PlayerLogic                    │  ← 玩家逻辑、金币
├─────────────────────────────────────┤
│  Mono 层（MonoBehaviour组件）         │
│  ├── BuildingMono                  │  ← 塔组件、状态机
│  ├── EnemyMono                      │  ← 敌人组件、寻路
│  └── StateMachine                   │  ← 通用状态机
├─────────────────────────────────────┤
│  View 层（表现层）                    │
│  ├── BuildingView                   │  ← 塔视图
│  ├── EnemyView                      │  ← 敌人视图
│  └── UI Framework                   │  ← UI框架
└─────────────────────────────────────┘
```

### 1.2 核心设计模式

#### **MVC/MVP 分离**
- **Logic（逻辑层）**：纯数据与逻辑计算，不依赖 Unity API，可独立测试
- **Mono（组件层）**：MonoBehaviour 组件，连接 Unity 生命周期
- **View（表现层）**：负责视觉效果与 UI 交互

#### **管理器模式**
各个子系统通过 Manager 统一管理：
- `BuildingManager`：管理所有塔的创建、更新、回收，处理范围检测
- `EnemyManager`：管理敌人生成、回收、状态更新
- `PlayerManager`：管理玩家数据、金币、生命值
- `BuffManager`：管理全局 Buff 数据、商店临时 Buff

#### **状态机模式**
塔和敌人使用状态机控制行为：
- **塔状态**：`IdleMode/IdleState` ↔ `AttackMode/AttackState`
- **敌人状态**：`IdleState` → `MoveState` → `AttackState`
- **子弹状态**：`BulletFlyState` → `BulletTrackState` → `BulletAttackState`

#### **对象池模式**
使用 `GameObjectPool` 管理子弹和敌人的创建与回收，避免频繁实例化造成的性能问题。

#### **单例模式**
关键管理器使用单例模式：
- `GameManager`：使用 `Singleton<GameManager>`
- `ArchiveManager`：使用静态 Instance，支持跨场景持久化（`DontDestroyOnLoad`）

---

## 2. 核心系统详解

### 2.1 Buff 系统

#### 2.1.1 架构设计

Buff 系统采用**模块化设计**，核心组件包括：

- **BuffData**：Buff 配置数据（ScriptableObject）
- **BuffInfo**：Buff 运行时实例
- **BuffHandler**：Buff 管理器，负责添加、移除、更新
- **BaseBuffModule**：Buff 效果模块基类

```csharp
// BuffData 配置示例
[CreateAssetMenu(fileName = "_BuffData", menuName = "BuffSystem/BuffData")]
public class BuffData : ScriptableObject
{
    public int id;                    // 唯一标识
    public string buffName;           // Buff名称
    public int priority;              // 优先级（用于排序）
    public int maxStack;              // 最大层数
    public bool isForever;           // 是否永久
    public float duration;            // 持续时间
    public float tickTime;           // 触发间隔
    public BuffUpdateTimeEnum buffUpdateTime;      // 叠加方式
    public BuffRemoveStackUpdateEnum buffRemoveStackUpdate; // 移除方式
    public List<BaseBuffModule> OnCreate;  // 创建时回调
    public List<BaseBuffModule> OnRemove;   // 移除时回调
    public List<BaseBuffModule> OnTick;     // 触发时回调
}
```

#### 2.1.2 层数与冲突规则

**层数机制**：
- 同类型 Buff 可叠加，通过 `curStack` 记录当前层数
- 达到 `maxStack` 后不再叠加，但可刷新持续时间
- 支持永久 Buff（`isForever = true`）

**冲突处理**：
- **相同 Buff**：层数叠加，持续时间根据 `buffUpdateTime` 更新：
  - `Add`：累加持续时间（新持续时间 = 原持续时间 + 新持续时间）
  - `Replace`：重置持续时间（新持续时间 = 新持续时间）
  - `Keep`：保持原持续时间（不更新）
- **不同 Buff**：可同时存在，通过优先级排序执行（`priority` 降序）

**移除规则**：
- `Clear`：移除时直接清空整个 Buff（触发 OnRemove 后移除）
- `Reduce`：移除时减少一层，层数为 0 时移除（触发 OnRemove 后减层）

**排序机制**：
- 使用**插入排序**算法维护链表，按 `priority` 降序排列
- 新 Buff 添加时自动插入到正确位置

#### 2.1.3 Buff 模块类型

项目实现了多种 Buff 模块：

| 模块类型 | 功能 | 适用对象 | 回调点 |
|---------|------|---------|--------|
| `ModifyBuildingInfoBuffModule` | 修改塔属性（攻击力、范围、攻速等） | 塔 | OnCreate/OnRemove |
| `ModifyEnemyInfoBuffModule` | 修改敌人属性（血量、速度、护甲等） | 敌人 | OnCreate/OnRemove |
| `Burning` | 持续灼烧伤害（DoT） | 敌人 | OnTick |
| `FrenzyOn` | 狂暴效果（半血触发） | 敌人 | OnCreate |
| `ModifyPenetrateNumBuffModule` | 修改穿透数量 | 塔 | OnCreate/OnRemove |
| `ModifySputterRadiusBuffModule` | 修改溅射范围 | 塔 | OnCreate/OnRemove |

#### 2.1.4 Buff 执行流程

```
1. BuffHandler.AddBuff() 添加 Buff
   ├── 检查是否已存在相同 Buff（FindBuff）
   ├── 存在且未满层：层数+1，更新持续时间（根据 buffUpdateTime）
   ├── 存在且满层：仅更新持续时间
   └── 不存在：创建 BuffInfo，插入排序列表（按 priority）

2. BuffHandler.Tick() 每帧更新
   ├── 遍历所有 Buff
   ├── 检查 OnTick 回调（如 Burning）
   │   └── tickTimer <= 0 时触发，重置 tickTimer
   ├── 更新 tickTimer 和 durationTimer
   └── 移除过期 Buff（durationTimer <= 0）

3. BuffHandler.RemoveBuff() 移除 Buff
   ├── 根据移除规则（Clear/Reduce）处理
   └── 触发 OnRemove 回调
```

#### 2.1.5 商店临时 Buff

通过 `BuffManager.enemyAdditionalBuffs` 管理商店购买的临时 Buff：

```csharp
// BuffManager.AddAdditionalEnemyBuff()
var add = new AdditionalBuff(buffData);  // 30秒限时（由 buffData.duration 决定）
enemyAdditionalBuffs.Add(add);

// 敌人生成时自动挂载
foreach (var addBuff in GameManager.Instance.buffManager.enemyAdditionalBuffs)
{
    mono.enemyLogic.BuffHandler.AddBuff(addBuff.GetBuffInfo(mono.gameObject), true);
}
```

---

### 2.2 属性系统（ValueChannel）

#### 2.2.1 设计原理

`ValueChannel` 是项目的核心属性计算系统，实现了**加法修饰符 + 乘法修饰符**的复合计算：

```csharp
最终值 = (基础值 + 加法总和) × 乘法总积
```

**优势**：
- 支持多个 Buff 同时修改同一属性
- 加法与乘法分离，避免数值爆炸
- 自动触发 `OnValueChanged` 事件，便于响应式更新
- 支持运行时修改基础值（`SetBaseValue`）

#### 2.2.2 使用示例

```csharp
// 创建属性通道
ValueChannel attack = new ValueChannel(100f);  // 基础攻击力 100

// Buff 1：加算 +50
attack.ModifyAdditive(50f);  // 当前值 = (100 + 50) × 1 = 150

// Buff 2：乘算 ×1.5
attack.ModifyMultiplier(1.5f);  // 当前值 = (100 + 50) × 1.5 = 225

// Buff 3：加算 +30
attack.ModifyAdditive(30f);  // 当前值 = (100 + 80) × 1.5 = 270
```

#### 2.2.3 在 Buff 中的应用

Buff 模块通过 `ValueChannel` 修改属性：

```csharp
// ModifyBuildingInfoBuffModule.Apply()
if(buildingProperty.attack != Vector2.zero)
{
    info.attack.ModifyAdditive(buildingProperty.attack.x);    // x = 加算值
    info.attack.ModifyMultiplier(buildingProperty.attack.y); // y = 乘算值
}
```

**Vector2 格式**：
- `x`：加法修饰符（加算值）
- `y`：乘法修饰符（乘算值，通常为 1.0 表示无变化）

---

### 2.3 伤害计算系统

#### 2.3.1 伤害计算流程

```
1. 塔攻击
   ├── 获取塔的攻击力（已通过 Buff 修改，attack.Value）
   └── 创建子弹或直接造成伤害

2. 子弹命中敌人
   ├── 调用 EnemyLogic.ModifyCurrentHealth(-damage)

3. 敌人伤害处理（EnemyLogic.ModifyCurrentHealth）
   ├── 应用伤害吸收率：damage × atkAbsorbPercent.Value
   ├── 计算护甲：min(0, shield.Value + 吸收后伤害)
   └── 扣除血量：curHealth += finalDamage
```

#### 2.3.2 伤害计算公式

```csharp
// 伪代码
基础伤害 = 塔攻击力.Value
吸收后伤害 = 基础伤害 × 敌人伤害吸收率.Value
护甲减免 = min(0, shield.Value + 吸收后伤害)  // 护甲为负数时生效
最终伤害 = max(0, 吸收后伤害 + 护甲减免)  // 确保伤害非负
```

**精英怪抗性**：
通过 `atkAbsorbPercent` 实现，精英怪对特定 Buff 的伤害减半（如 `atkAbsorbPercent = 0.5`）。

#### 2.3.3 DoT 伤害（持续伤害）

通过 `Burning` Buff 模块实现：

```csharp
// Burning.Apply() - OnTick 回调
float finalPercent = buffInfo.curStack * percent;  // 层数 × 百分比
float damage = finalPercent / 100 * maxHealth.Value;
enemyLogic.ModifyCurrentHealth(-damage);
```

**特点**：
- 每 `tickTime` 触发一次
- 伤害基于最大生命值百分比
- 支持层数叠加（每层增加百分比）

---

### 2.4 塔系统

#### 2.4.1 三种塔类型

| 塔类型 | 特点 | 状态机 | 特殊能力 |
|--------|------|--------|---------|
| **SingleTower** | 单体高伤 | `SingleTowerIdleMode` ↔ `SingleTowerAttackMode` | 穿透、溅射、子弹追踪 |
| **AoeTower** | 群体伤害 | `AoeTowerIdleState` ↔ `AoeTowerAttackState` | 范围攻击 |
| **DotTower** | 持续灼烧 | `DotTowerIdleState` ↔ `DotTowerAttackState` | DoT 伤害（Burning Buff） |

#### 2.4.2 塔升级系统

```csharp
// BuildingLogic.LevelUp()
public bool LevelUp()
{
    if (!buildingInfo.LevelUp()) return false;
    
    // 升级时添加新 Buff
    foreach (var buffData in buildingInfo.levelData[buildingInfo.curLv].addBuffs)
    {
        BuffHandler.AddBuff(new BuffInfo(buffData, m_MonoGameObject, m_MonoGameObject));
    }
    return true;
}
```

**升级机制**：
- 每个等级有独立的 `BuildingLevelData` 配置
- 升级时重置基础值（`SetBaseValue`）
- 升级时自动添加该等级的 Buff（`addBuffs`）
- 升级费用通过 `upgradeCost` 获取

#### 2.4.3 DPS 预估系统

系统提供 DPS（每秒伤害）预估功能，用于 UI 显示：

```csharp
// BuildingInfo.EstimateDPS()
public float EstimateDPS(int targetCount = 1)
{
    if (attackInterval.Value <= 0) return 0f;
    
    float baseDPS = attack.Value / attackInterval.Value;
    
    if (ifSingle)
    {
        // 单体攻击：考虑同时攻击的目标数量
        return baseDPS * Mathf.Min(attackNum.Value, targetCount);
    }
    else
    {
        // 范围攻击：攻击所有目标
        return baseDPS * targetCount;
    }
}
```

#### 2.4.4 范围检测优化

使用 **Compute Shader** 优化塔的范围检测：

**GPU 并行计算流程**：
```
1. 准备数据：塔位置、范围、敌人位置
2. 上传到 GPU ComputeBuffer
3. 执行 ComputeShader 计算距离矩阵
4. 读取结果，更新每个塔的 enemiesInRange 列表
```

**Compute Shader 实现**：
```hlsl
// BuildingEnemyRange.compute
[numthreads(64,1,1)]
void CalculateEnemyInRange(uint3 id : SV_DispatchThreadID)
{
    uint index = id.x;
    uint buildingIndex = index / EnemyCount;
    uint enemyIndex = index % EnemyCount;
    
    // 计算距离（xz平面）
    float2 delta = EnemyPositions[enemyIndex] - BuildingPositions[buildingIndex];
    float distanceSq = dot(delta, delta);
    float rangeSq = BuildingRanges[buildingIndex] * BuildingRanges[buildingIndex];
    
    // 将结果写入矩阵
    EnemyInRangeMatrix[index] = (distanceSq <= rangeSq) ? 1 : 0;
}
```

**性能优势**：
- CPU 计算：O(塔数 × 敌人数)，单线程顺序计算
- GPU 计算：并行计算，64 个线程一组，大幅提升性能
- 回退机制：ComputeShader 失败时自动回退到 CPU 计算（`FallbackToCPUCalculation`）

**定时更新**：
- 范围检测每 `ConstManager.k_UpdateInterval`（0.2 秒）更新一次
- 减少不必要的计算，提升性能

---

### 2.5 敌人系统

#### 2.5.1 寻路系统

使用 **Unity NavMesh** 实现寻路：

```csharp
// MoveState.OnEnter()
m_Agent.SetDestination(GameManager.Instance.playerManager.playerPos);

// MoveState.OnTick()
if (m_Agent.remainingDistance <= m_Agent.stoppingDistance)
{
    stateMachine.SwitchTo<AttackState>();  // 到达目标，攻击
}
```

**特点**：
- 使用 `NavMeshAgent` 组件
- 目标点为玩家建筑位置（`playerManager.playerPos`）
- 支持路径拐弯、障碍物避让

#### 2.5.2 精英怪系统

精英怪通过配置实现特殊抗性：

```csharp
// EnemyData
[LabelText("伤害吸收倍率"), Range(0.1f, 2f)]
public float atkAbsorbPercent = 1f;  // 普通怪 = 1，精英怪 = 0.5（减半）
```

**伤害计算**：
- 普通怪：`atkAbsorbPercent = 1.0`，全额伤害
- 精英怪：`atkAbsorbPercent = 0.5`，伤害减半

#### 2.5.3 敌人状态机

```
IdleState → MoveState → AttackState
   ↑           ↓           ↓
   └───────────┴───────────┘
```

**状态说明**：
- `IdleState`：初始状态，等待开始
- `MoveState`：寻路移动，使用 NavMeshAgent
- `AttackState`：攻击玩家建筑

#### 2.5.4 狂暴机制

通过 `FrenzyOn` Buff 模块实现半血触发：

```csharp
// EnemyLogic.ModifyCurrentHealth()
if (EnemyInfo.curHealth < EnemyInfo.maxHealth.Value / 2 && !firstDropDownHalfHealth)
{
    firstDropDownHalfHealth = true;
    OnFirstDropDownHalf?.Invoke();  // 触发狂暴 Buff
}
```

---

### 2.6 波次系统

#### 2.6.1 数据结构

```csharp
// EnemyWaveDatas
public class EnemyWaveData
{
    public float waitTime;              // 波次开始前等待时间
    public List<EnemyWaveSingle> singleWaveList;  // 小波次列表
    public float interval;              // 小波次间隔
}

public class EnemyWaveSingle
{
    [StringToEnum("Enemy")] public string enemyName;  // 敌人类型（枚举转换）
    public int num;                    // 数量
    public float singleInterval;       // 单个间隔
    [StringToEnum("Enemy Spawner")] public string spawnerID;  // 出怪口ID（枚举转换）
}
```

#### 2.6.2 时间轴生成

系统自动生成波次时间轴，用于 UI 显示：

```csharp
// EnemyWaveDatas.GenerateTimeline()
WaveTimeline timeline = new WaveTimeline();
float currentTime = 0f;

for (int waveIndex = 0; waveIndex < waveDataList.Count; waveIndex++)
{
    // 波次开始前等待
    currentTime += waveData.waitTime;
    
    // 生成敌人
    for (int spawnIndex = 0; spawnIndex < waveData.singleWaveList.Count; spawnIndex++)
    {
        for (int enemyIndex = 0; enemyIndex < spawnData.num; enemyIndex++)
        {
            float enemySpawnTime = currentTime + (enemyIndex * spawnData.singleInterval);
            // 添加事件到时间轴
        }
        currentTime += waveData.interval;  // 组间隔
    }
}

return timeline;
```

**时间轴事件类型**：
- `WAVE_START`：波次开始
- `WAVE_END`：波次结束
- `GROUP_INTERVAL`：组间隔
- 敌人生成事件：包含敌人名称、出怪口、绝对时间等信息

#### 2.6.3 JSON 配置

支持 JSON 导入/导出，实现数据驱动：

**编辑器工具**：
- `EnemyWaveDataEditorWindow`：可视化波次编辑器
- 自定义 Inspector：快速导入/导出 JSON
- Debug 时间轴：一键生成时间轴日志

**JSON 格式**：
```json
{
  "waveDataList": [
    {
      "waitTime": 5.0,
      "singleWaveList": [
        {
          "enemyName": "NormalEnemy",
          "num": 10,
          "singleInterval": 0.5,
          "spawnerID": "Spawner1"
        },
        {
          "enemyName": "EliteEnemy",
          "num": 3,
          "singleInterval": 1.0,
          "spawnerID": "Spawner2"
        }
      ],
      "interval": 2.0
    }
  ]
}
```

---

### 2.7 存档系统

#### 2.7.1 存档数据结构

```csharp
// ArchiveData
public class SinglePlayerArchiveData
{
    public int maxWave = 0;  // 最高波次
}

// 塔解锁逻辑（BuildingDatas.GetUnLockedBuildingDataList）
public List<BuildingData> GetUnLockedBuildingDataList()
{
    var wave = ArchiveManager.Instance.data.GetCurPlayer().maxWave;
    return buildingDataList.Where(x => x.unlockWaveNum <= wave).ToList();
}
```

#### 2.7.2 解锁机制

**解锁条件**：
- 塔通过 `unlockWaveNum` 配置解锁条件
- 达到指定波次后自动解锁对应塔
- 存档持久化（ScriptableObject）

**存档更新**：
```csharp
// GameManager.UpdateArchive()
ArchiveManager.Instance.UpdateCurPlayerMaxWave(
    win ? enemyManager.enemySpawner.GetCurrentWaveIndex() + 1
        : enemyManager.enemySpawner.GetCurrentWaveIndex()
);
```

**特点**：
- 支持多玩家存档（`List<SinglePlayerArchiveData>`）
- 跨场景持久化（`DontDestroyOnLoad`）
- 自动保存最高波次记录

---

### 2.8 商店系统

#### 2.8.1 临时 Buff 购买

商店购买 Buff 后，通过 `BuffManager.enemyAdditionalBuffs` 管理：

```csharp
// BuffManager.AddAdditionalEnemyBuff()
var add = new AdditionalBuff(buffData);  // 限时 30s（由 buffData.duration 决定）
enemyAdditionalBuffs.Add(add);

// 敌人生成时自动挂载
foreach (var addBuff in GameManager.Instance.buffManager.enemyAdditionalBuffs)
{
    mono.enemyLogic.BuffHandler.AddBuff(addBuff.GetBuffInfo(mono.gameObject), true);
}
```

**特点**：
- 购买后所有新生成的敌人都会获得该 Buff
- 持续时间通过 `AdditionalBuff` 管理
- 时间到期后自动移除（`OnTimeUp` 事件）

---

### 2.9 网格搭建辅助器系统

#### 2.9.1 功能概述

网格搭建辅助器（`PositionSnapping`）是一个编辑器工具，用于在场景编辑时自动对齐物体到网格，并提供可视化网格显示和防重叠功能。

#### 2.9.2 核心功能

**网格对齐**：
- 自动将物体位置对齐到网格（`ConstManager.k_GridSize`，默认 100 单位）
- 支持 XZ 平面的网格对齐
- 自动设置 Y 轴偏移（`k_PreviewYOffset`，默认 0.06f）

**网格可视化**：
- 使用 Gizmos 在 Scene 视图中绘制网格线
- 高亮显示当前物体所在的格子（绿色边框）
- 可配置网格颜色、显示范围

**防重叠检测**：
- 使用物理碰撞检测（`Physics.OverlapBox`）检查重叠
- 自动寻找不重叠的位置（向右偏移）
- 支持自定义物体占用大小和检测层

#### 2.9.3 实现细节

**编辑器模式支持**：
```csharp
[ExecuteInEditMode]
public class PositionSnapping : MonoBehaviour
{
    // 订阅编辑器更新事件
    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }
    
    private void OnEditorUpdate()
    {
        if (!Application.isPlaying && transform.position != m_LastPosition)
        {
            SnapPosition();
            m_LastPosition = transform.position;
        }
    }
}
```

**网格对齐算法**：
```csharp
private void SnapPosition()
{
    var currentPosition = transform.position;
    var snappedPosition = new Vector3(
        Mathf.Round(currentPosition.x / m_SnapStep.x) * m_SnapStep.x,
        k_PreviewYOffset,
        Mathf.Round(currentPosition.z / m_SnapStep.y) * m_SnapStep.y
    );
    
    // 防重叠处理
    if (enableAntiOverlap)
    {
        snappedPosition = FindNonOverlappingPosition(snappedPosition);
    }
    
    transform.position = snappedPosition;
}
```

**防重叠检测**：
```csharp
private bool IsOverlapping(Vector3 position)
{
    // 使用物理碰撞检测
    Collider[] colliders = Physics.OverlapBox(
        position + Vector3.up * objectSize.y * 0.5f,
        objectSize * 0.5f,
        Quaternion.identity,
        overlapCheckLayer
    );
    
    // 检查是否有其他 PositionSnapping 物体重叠
    return colliders.Any(collider => 
        collider.gameObject != gameObject && 
        collider.GetComponent<PositionSnapping>() != null
    );
}
```

**网格可视化（Gizmos）**：
```csharp
private void OnDrawGizmos()
{
    // 绘制网格线（X 和 Z 方向）
    // 高亮当前格子（绿色边框）
    // 显示物体占用区域（红色线框）
}
```

#### 2.9.4 配置参数

| 参数 | 说明 | 默认值 |
|------|------|--------|
| `m_SnapStep` | 网格对齐步长 | `(100, 100)` |
| `k_PreviewYOffset` | Y 轴偏移 | `0.06f` |
| `showGrid` | 是否显示网格 | `true` |
| `gridColor` | 网格颜色 | `(0.5, 0.5, 0.5, 0.3)` |
| `gridSize` | 网格显示范围 | `10` |
| `enableAntiOverlap` | 是否启用防重叠 | `true` |
| `overlapCheckLayer` | 重叠检测层 | `-1`（所有层） |
| `objectSize` | 物体占用大小 | `(1, 1, 1)` |

#### 2.9.5 使用场景

**适用场景**：
- 场景编辑时快速对齐物体到网格
- 塔防游戏中建造地块的网格对齐
- 需要精确网格布局的场景搭建

**使用方法**：
1. 将 `PositionSnapping` 组件挂载到需要对齐的物体上
2. 在编辑器中移动物体，位置会自动对齐到网格
3. 启用防重叠功能可避免物体重叠
4. 在 Scene 视图中查看网格可视化效果

**特点**：
- 仅在编辑器模式下生效（`ExecuteInEditMode`）
- 支持 Undo 操作（`Undo.RecordObject`）
- 可视化反馈，便于调试和布局

---

## 3. 技术亮点

### 3.1 性能优化

#### **Compute Shader 范围检测**
- 使用 GPU 并行计算塔与敌人的距离矩阵
- 64 线程组并行处理，大幅提升 200+ 敌人同屏时的性能
- 自动回退机制：ComputeShader 失败时回退到 CPU 计算

#### **对象池**
- 子弹和敌人使用对象池，避免频繁创建/销毁
- 减少 GC 压力，提升性能

#### **定时更新**
- 范围检测每 0.2 秒更新一次（`ConstManager.k_UpdateInterval`）
- 减少不必要的计算，提升帧率

#### **优化策略**
- 使用平方距离避免开方运算（Compute Shader）
- 空值检查和异常处理，确保稳定性

### 3.2 数据驱动架构

#### **ScriptableObject 配置**
- 塔、敌人、Buff 数据全部通过 ScriptableObject 配置
- 运行时加载，无需硬编码
- 支持编辑器可视化编辑

#### **JSON 波次配置**
- 编辑器工具支持 JSON 导入/导出
- 支持自定义波次方案
- 时间轴自动生成，便于调试

#### **枚举转换工具**
- 使用 `StringToEnum` 属性实现字符串到枚举的转换
- 波次配置中自动验证敌人类型和出怪口

### 3.3 编辑器工具

#### **EnemyWaveDataEditorWindow**
- 可视化波次编辑器窗口
- JSON 一键导入/导出
- 时间轴预览与调试
- 数据预览功能

#### **自定义 Inspector**
- 快速导出/导入 JSON 按钮
- Debug 时间轴功能
- 打开编辑器窗口按钮

#### **StringToEnum 工具**
- 自动字符串到枚举转换
- 配置验证和错误提示

#### **网格搭建辅助器**
- **PositionSnapping**：编辑器网格对齐工具
- 自动对齐物体到网格，支持可视化网格显示
- 防重叠检测，避免物体重叠
- 实时反馈，便于场景搭建

### 3.4 状态机架构

#### **通用状态机**
- `StateMachine` 组件可挂载到任意 GameObject
- 支持状态注册、切换、生命周期管理
- 类型安全的状态切换（`SwitchTo<T>()`）

#### **模块化状态**
- 每个状态独立实现 `IState` 接口
- 生命周期方法：`OnEnter`、`OnTick`、`OnExit`
- 便于扩展和维护

### 3.5 UI 框架

#### **UI 管理器**
- `UIMgr` 单例管理所有 UI 面板
- 支持 UI 创建、获取、销毁
- 自动层级管理

#### **UI 组件**
- `UIButtonEx`：扩展按钮组件
- `UIList`：列表组件
- `HorizontalPageView`：横向页面视图
- `IrregularShapeButton`：不规则形状按钮

---

## 4. 数据格式样例

### 4.1 塔数据（ScriptableObject）

```csharp
// BuildingData
{
    buildingName: "SingleTower",
    levelData: [
        {
            cost: 100,              // 建造费用
            attack: 50,             // 攻击力
            attackRange: 5.0,       // 攻击范围
            attackInterval: 1.0,    // 攻击间隔
            ifSingle: true,         // 单体攻击
            attackNum: 3,           // 同时攻击目标数
            giveBack: 50,           // 拆除返还
            addBuffs: [/* BuffData 列表 */]
        },
        {
            cost: 150,              // 升级费用
            attack: 80,             // 升级后攻击力
            attackRange: 6.0,       // 升级后范围
            // ...
        }
    ],
    unlockWaveNum: 5,              // 第5波解锁
    prefab: /* GameObject */
}
```

### 4.2 敌人数据（ScriptableObject）

```csharp
// EnemyData
{
    enemyName: "NormalEnemy",
    maxHealth: 100,
    speed: 2.0,
    attack: 10,
    atkAbsorbPercent: 1.0,          // 普通怪 = 1，精英怪 = 0.5
    shield: 0,                      // 护甲
    value: 10,                      // 击杀奖励
    initBuffs: [/* 初始 Buff */],
    prefab: /* GameObject */
}
```

### 4.3 Buff 数据（ScriptableObject）

```csharp
// BuffData
{
    id: 1,
    buffName: "攻击加速",
    priority: 10,
    maxStack: 5,
    isForever: false,
    duration: 10.0,
    tickTime: 0,                    // OnTick 触发间隔
    buffUpdateTime: Add,            // 叠加时累加时间
    buffRemoveStackUpdate: Reduce,  // 移除时减层
    OnCreate: [ModifyBuildingInfoBuffModule],  // 创建时修改攻击间隔
    OnRemove: [/* 移除回调 */],
    OnTick: [/* 触发回调 */]
}
```

### 4.4 波次数据（JSON）

```json
{
  "waveDataList": [
    {
      "waitTime": 5.0,
      "singleWaveList": [
        {
          "enemyName": "NormalEnemy",
          "num": 10,
          "singleInterval": 0.5,
          "spawnerID": "Spawner1"
        },
        {
          "enemyName": "EliteEnemy",
          "num": 3,
          "singleInterval": 1.0,
          "spawnerID": "Spawner2"
        }
      ],
      "interval": 2.0
    }
  ]
}
```

### 4.5 存档数据（ScriptableObject）

```csharp
// ArchiveData
{
    players: [
        {
            maxWave: 10  // 最高波次
        }
    ],
    curPlayerDataIndex: 0
}
```

---

## 5. Buff/伤害计算顺序

### 5.1 Buff 应用顺序

```
1. Buff 添加时（BuffHandler.AddBuff）
   ├── 检查是否已存在（FindBuff）
   ├── 存在且未满层：层数+1，更新持续时间（根据 buffUpdateTime）
   ├── 存在且满层：仅更新持续时间
   └── 不存在：创建 BuffInfo，插入排序列表（按 priority 降序）

2. Buff 排序
   └── 使用插入排序，按 priority 降序排列

3. Buff 效果应用
   ├── OnCreate：创建时立即应用（属性修改）
   ├── OnTick：每 tickTime 触发一次（如 Burning）
   └── OnRemove：移除时应用（属性恢复）
```

### 5.2 属性计算顺序

```
最终属性值 = (基础值 + 所有加法修饰符) × 所有乘法修饰符的积

示例：
基础攻击力 = 100
Buff1: +50 (加法)
Buff2: ×1.5 (乘法)
Buff3: +30 (加法)

最终攻击力 = (100 + 50 + 30) × 1.5 = 270
```

**计算特点**：
- 所有加法修饰符先累加
- 所有乘法修饰符再相乘
- 计算顺序不影响结果（数学结合律）

### 5.3 伤害计算顺序

```
1. 塔攻击力计算
   └── attack.Value = (baseAttack + additive) × multiplier

2. 敌人伤害处理（EnemyLogic.ModifyCurrentHealth）
   ├── 基础伤害 = 塔攻击力.Value
   ├── 伤害吸收 = 基础伤害 × atkAbsorbPercent.Value
   ├── 护甲计算 = min(0, shield.Value + 伤害吸收)
   └── 最终伤害 = 伤害吸收 + 护甲计算（最小为 0）

3. 血量扣除
   └── curHealth -= 最终伤害
```

**详细公式**：
```csharp
// 伪代码
基础伤害 = 塔攻击力.Value
吸收后伤害 = 基础伤害 × 敌人伤害吸收率.Value
护甲减免 = min(0, shield.Value + 吸收后伤害)  // 护甲为负数时生效
最终伤害 = max(0, 吸收后伤害 + 护甲减免)  // 确保伤害非负
```

### 5.4 DoT 伤害计算

```
1. Burning Buff 触发（OnTick）
   ├── finalPercent = buffInfo.curStack × percent
   └── damage = finalPercent / 100 × maxHealth.Value

2. 应用伤害
   └── enemyLogic.ModifyCurrentHealth(-damage)
```

**特点**：
- 基于最大生命值百分比
- 支持层数叠加
- 每 `tickTime` 触发一次

---

## 6. 扩展点

### 6.1 新增塔类型

1. **创建新的塔类**（继承 `BuildingMono`）
   ```csharp
   public class NewTower : BuildingMono
   {
       // 实现自定义逻辑
   }
   ```

2. **实现自定义状态机**
   - 创建 `XXXIdleState`、`XXXAttackState`
   - 实现 `IState` 接口

3. **在 BuildingBuilder 中注册新塔**
   ```csharp
   // BuildingBuilder.CreateBuilding()
   switch (buildingName)
   {
       case "NewTower":
           // 创建新塔
           break;
   }
   ```

4. **配置 BuildingData ScriptableObject**
   - 设置基础属性
   - 配置升级数据
   - 设置解锁条件

### 6.2 新增 Buff 模块

1. **创建新的 Buff 模块**（继承 `BaseBuffModule`）
   ```csharp
   [CreateAssetMenu(fileName = "_NewBuffModule", 
       menuName = "BuffSystem/BuffModule/NewBuffModule")]
   public class NewBuffModule : BaseBuffModule
   {
       public override void Apply(BuffInfo buffInfo)
       {
           // 实现自定义效果
           // 可以通过 buffInfo.target 获取目标组件
           // 可以通过 buffInfo.curStack 获取当前层数
       }
   }
   ```

2. **在 BuffData 中配置使用**
   - 添加到 `OnCreate`、`OnRemove` 或 `OnTick` 列表

### 6.3 新增敌人类型

1. **创建新的敌人类**（继承 `EnemyMono`）
   ```csharp
   public class NewEnemy : EnemyMono
   {
       // 实现自定义逻辑
   }
   ```

2. **实现自定义状态**（如特殊技能状态）
   - 创建新状态类，实现 `IState` 接口

3. **配置 EnemyData ScriptableObject**
   - 设置基础属性
   - 配置初始 Buff
   - 设置精英怪抗性（`atkAbsorbPercent`）

4. **在波次配置中使用**
   - 在 `EnemyWaveSingle.enemyName` 中指定敌人名称

### 6.4 新增波次事件

1. **扩展 WaveEvent 类型**
   ```csharp
   // EnemyWaveDatas.WaveTimeline.WaveEvent
   public enum WaveEventType
   {
       WAVE_START,
       WAVE_END,
       GROUP_INTERVAL,
       NEW_EVENT_TYPE  // 新增事件类型
   }
   ```

2. **在 GenerateTimeline() 中生成新事件**
   ```csharp
   timeline.events.Add(new WaveTimeline.WaveEvent
   {
       waveIndex = waveIndex,
       enemyName = "NEW_EVENT_TYPE",
       // ...
   });
   ```

3. **在 UI 中显示新事件**
   - 修改 `UIGameInfo` 中的时间轴显示逻辑

### 6.5 新增伤害类型

1. **在 EnemyLogic.ModifyCurrentHealth() 中添加伤害类型判断**
   ```csharp
   public float ModifyCurrentHealth(float delta, DamageType damageType = DamageType.Normal)
   {
       switch (damageType)
       {
           case DamageType.Normal:
               // 普通伤害计算
               break;
           case DamageType.Fire:
               // 火焰伤害计算
               break;
       }
   }
   ```

2. **实现不同的伤害计算公式**
   - 不同伤害类型可能有不同的抗性计算

3. **在塔或 Buff 中指定伤害类型**
   - 修改攻击逻辑，传递伤害类型参数

### 6.6 扩展 UI 功能

1. **创建新的 UI 面板**
   ```csharp
   public class UINewPanel : UIFormBase
   {
       public override void Init()
       {
           // 初始化逻辑
       }
   }
   ```

2. **在 UIMgr 中注册**
   - 使用 `UIMgr.Instance.CreateUI<UINewPanel>()` 创建

3. **实现 UI 交互逻辑**
   - 使用 `UIButtonEx` 等组件
   - 实现事件回调

### 6.7 扩展网格搭建辅助器

1. **自定义对齐算法**
   ```csharp
   // 在 PositionSnapping 中扩展对齐规则
   private Vector3 CustomSnapPosition(Vector3 position)
   {
       // 实现自定义对齐逻辑（如六边形网格、不规则网格等）
       return position;
   }
   ```

2. **扩展防重叠策略**
   - 修改 `FindNonOverlappingPosition` 方法
   - 实现更复杂的搜索算法（如螺旋搜索、最近空位搜索等）

3. **增强可视化效果**
   - 在 `OnDrawGizmos` 中添加更多可视化元素
   - 支持不同网格类型（六边形、三角形等）

4. **运行时支持**
   - 移除 `#if UNITY_EDITOR` 条件，支持运行时网格对齐
   - 添加运行时网格对齐 API

---

## 7. 调试工具

### 7.1 Debug Overlay

通过 `UIDebug` 显示：
- **FPS**：实时帧率（每 0.2 秒更新）
- **存活敌人数**：`EnemyManager.activeEnemies.Count`
- **活跃 Buff 数**：`GameManager.GetActiveBuffNum()`

**实现**：
```csharp
// UIDebug.Update()
fpsText.text = $"FPS: {1f / Time.deltaTime:F1}";
enemyCountText.text = $"Enemies: {GameManager.Instance.enemyManager.activeEnemies.Count}";
buffCountText.text = $"Buffs: {GameManager.Instance.GetActiveBuffNum()}";
```

### 7.2 编辑器工具

#### **波次编辑器**
- **路径**：`Tools/Enemy Wave Data Editor`
- **功能**：
  - 可视化编辑波次配置
  - JSON 一键导入/导出
  - 时间轴预览与调试
  - 数据预览

#### **自定义 Inspector**
- **快速导出/导入 JSON**：一键操作
- **Debug 时间轴**：生成详细的时间轴日志

#### **时间轴调试**
- 生成包含所有事件的时间轴日志
- 显示绝对时间、相对时间、事件描述

---

## 8. 性能指标

### 8.1 目标性能

- **同屏敌人**：200+ 敌人稳定运行
- **帧率**：100+ FPS
- **范围检测**：每 0.2 秒更新一次

### 8.2 优化措施

1. **Compute Shader**：GPU 并行计算范围检测
2. **对象池**：减少 GC 压力
3. **定时更新**：范围检测不每帧计算
4. **简化碰撞**：使用简化的碰撞体
5. **空值检查**：确保稳定性，避免异常

### 8.3 性能监控

- **Debug Overlay**：实时显示 FPS、敌人数量、Buff 数量
- **Unity Profiler**：分析性能瓶颈
- **异常处理**：Compute Shader 失败时自动回退

---

## 9. 代码规范

### 9.1 命名规范

- **类名**：PascalCase（如 `BuildingLogic`）
- **方法名**：PascalCase（如 `ModifyCurrentHealth`）
- **字段名**：camelCase（如 `buildingInfo`）
- **私有字段**：`m_` 前缀（如 `m_UpdateTimer`）
- **常量**：`k_` 前缀（如 `k_UpdateInterval`）

### 9.2 架构规范

- **Logic 层**：纯逻辑，不依赖 Unity API
- **Mono 层**：MonoBehaviour 组件，连接 Unity 生命周期
- **View 层**：负责视觉效果与 UI 交互
- **Manager 层**：统一管理子系统

### 9.3 代码组织

- **命名空间**：按模块划分（如 `Buildings`、`Enemy`、`Buff_System`）
- **文件组织**：按功能模块组织，便于维护
- **注释规范**：关键逻辑添加注释说明

---

## 10. 总结

本项目通过**数据驱动架构**、**模块化 Buff 系统**、**性能优化**等技术手段，实现了一个功能完整、易于扩展的塔防游戏 Demo。核心亮点包括：

1. ✅ **灵活的 Buff 系统**：支持层数、冲突规则、模块化扩展
2. ✅ **ValueChannel 属性系统**：加法与乘法修饰符分离计算
3. ✅ **Compute Shader 优化**：GPU 并行计算提升性能
4. ✅ **数据驱动**：ScriptableObject + JSON 配置
5. ✅ **编辑器工具**：波次 JSON 导入/导出、时间轴预览、网格搭建辅助器
6. ✅ **状态机架构**：清晰的行为控制
7. ✅ **对象池系统**：减少内存分配
8. ✅ **UI 框架**：模块化 UI 管理
9. ✅ **存档系统**：支持塔解锁、最高波次记录
10. ✅ **调试工具**：Debug Overlay、编辑器工具
11. ✅ **网格搭建辅助器**：自动对齐、可视化网格、防重叠检测

项目代码结构清晰，遵循 SOLID 原则，便于后续扩展和维护。通过合理的架构设计和性能优化，实现了 200+ 敌人同屏稳定运行的目标。

---

## 附录：关键常量

```csharp
// ConstManager
public const int k_GridSize = 100;                    // 网格大小
public static readonly int[] timeSettings = {1, 2, 3}; // 倍速设置
public const string k_StartSceneName = "Start";       // 开始场景
public const string k_GameSceneName = "Sample";      // 游戏场景
public const float k_UpdateInterval = 0.2f;          // 更新间隔
public const float k_FPSUpdateInterval = 0.2f;       // FPS 更新间隔
public const float k_PreviewYOffset = 0.06f;        // 预览 Y 偏移
```

---

**文档版本**：v1.0  
**最后更新**：2024  
**维护者**：开发团队

