using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Buff_System
{
    public class BuffHandler : MonoBehaviour
    {
        public LinkedList<BuffInfo> buffList = new LinkedList<BuffInfo>();

        private void Update()
        {
            BuffTickAndRemove();
        }

        private void BuffTickAndRemove()
        {
            var deleteBuffList = new List<BuffInfo>();
            
            foreach (var buffInfo in buffList)
            {
                // 先判断buff的触发
                if (buffInfo.buffData.OnTick != null)// 判断有没有Tick方法
                {
                    if (buffInfo.tickTimer < 0)// 如果Tick计时器到了
                    {
                        foreach (var buffModule in buffInfo.buffData.OnTick)
                        {
                            buffModule.Apply(buffInfo);
                        }
                        buffInfo.tickTimer = buffInfo.buffData.tickTime;// 重置计时器
                    }
                    else
                    {
                        buffInfo.tickTimer -= Time.deltaTime;// 继续倒计时
                    }
                }
                // 再判断buff的移除
                if (buffInfo.durationTimer < 0 && !buffInfo.buffData.isForever) // 持续时间归零则移除
                {
                    deleteBuffList.Add(buffInfo);
                }
                else // 否则继续倒计时
                {
                    buffInfo.durationTimer -= Time.deltaTime;
                }
            }
            
            foreach (var buffInfo in deleteBuffList)
            {
                RemoveBuff(buffInfo);
            }
        }

        /// <summary>
        /// 添加Buff
        /// </summary>
        /// <param name="buffInfo"></param>
        public void AddBuff(BuffInfo buffInfo)
        {
            var findBuffInfo = FindBuff(buffInfo.buffData.id); // 首先查找是否已经加过了
            if (findBuffInfo != null)   //如果存在
            {
                if (findBuffInfo.curStack < findBuffInfo.buffData.maxStack)// 如果还没到最大层数
                {
                    findBuffInfo.curStack++;
                    switch (findBuffInfo.buffData.buffUpdateTime)// 依据不同的更新方法更新计时器
                    {
                        case BuffUpdateTimeEnum.Add:
                            findBuffInfo.durationTimer += findBuffInfo.buffData.duration;
                            break;
                        case BuffUpdateTimeEnum.Replace:
                            findBuffInfo.durationTimer = findBuffInfo.buffData.duration;
                            break;
                    }

                    foreach (var buffModule in findBuffInfo.buffData.OnCreate)
                    {
                        buffModule.Apply(findBuffInfo);//触发创建Buff的回调点
                    }
                }
            }
            else    //如果不存在
            {
                buffInfo.durationTimer = buffInfo.buffData.duration;//将buff的时间设置为默认的最大时间
                // buffInfo.tickTimer = buffInfo.buffData.tickTime;//添加一这一行的话，加Buff的瞬间是不会触发OnTick的回调的
                foreach (var buffModule in buffInfo.buffData.OnCreate)
                {
                    buffModule.Apply(buffInfo);//触发创建Buff的回调点
                }
                buffList.AddLast(buffInfo);//加入链表
                //对Buff链表进行排序
                InsertionSortLinkedList(buffList);
            }
        }

        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="buffInfo"></param>
        public void RemoveBuff(BuffInfo buffInfo)
        {
            switch (buffInfo.buffData.buffRemoveStackUpdate)//判断buff的移除更新方式
            {
                case BuffRemoveStackUpdateEnum.Clear:
                    foreach (var buffModule in buffInfo.buffData.OnRemove)
                    {
                        buffModule.Apply(buffInfo);//触发移除buff的回调点
                    }
                    buffList.Remove(buffInfo);
                    break;
                case BuffRemoveStackUpdateEnum.Reduce:
                    buffInfo.curStack--;
                    foreach (var buffModule in buffInfo.buffData.OnRemove)
                    {
                        buffModule.Apply(buffInfo);//触发移除buff的回调点
                    }
                    if (buffInfo.curStack == 0)//如果删掉了最后一层
                    {
                        buffList.Remove(buffInfo);
                    }
                    else
                    {
                        buffInfo.durationTimer = buffInfo.buffData.duration;
                    }
                    break;
            }
        }

        private BuffInfo FindBuff(int buffDataID)
        {
            return buffList.FirstOrDefault(buffInfo => buffInfo.buffData.id == buffDataID);
        }
        
        // 插入排序算法
        private void InsertionSortLinkedList(LinkedList<BuffInfo> list)
        {
            if (list?.First == null) return;//链表为空或只有一个元素时无需排序

            var current = list.First.Next;
            
            while (current != null)
            {
                var next = current.Next;
                var prev = current.Previous;

                while (prev != null && prev.Value.buffData.priority > current.Value.buffData.priority)
                {
                    prev = prev.Previous;
                }

                if (prev == null)
                {
                    // current应该成为新的头节点
                    list.Remove(current);
                    list.AddFirst(current);
                }
                else
                {
                    // 将current插入到prev之后
                    list.Remove(current);
                    list.AddAfter(prev, current);
                }

                current = next;
            }
        }

    }
}