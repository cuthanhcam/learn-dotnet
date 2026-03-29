using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkedLists
{
    public class MergeTwoListsSolution
    {
        public ListNode MergeTwoLists(ListNode l1, ListNode l2)
        {
            ListNode dummy = new ListNode(0);
            ListNode current = dummy;

            while (l1 != null && l2 != null)
            {
                if (l1.val <= l2.val)
                {
                    current.next = l1;
                    l1 = l1.next;
                }
                else
                {
                    current.next = l2;
                    l2 = l2.next;
                }
                current = current.next;
            }

            current.next = l1 != null ? l1 : l2;
            return dummy.next;
        }

        public ListNode MergeTwoListsRecursive(ListNode l1, ListNode l2)
        {
            if (l1 == null) return l2;
            if (l2 == null) return l1;
            if (l1.val <= l2.val)
            {
                l1.next = MergeTwoListsRecursive(l1.next, l2);
                return l1;
            }
            l2.next = MergeTwoListsRecursive(l1, l2.next);
            return l2;
        }
    }
}