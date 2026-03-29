using System;

namespace LinkedLists
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test ReverseLinkedList
            ListNode head = new ListNode(1, new ListNode(2, new ListNode(3)));
            var reverse = new ReverseLinkedListSolution();
            ListNode reversed = reverse.ReverseList(head);
            Console.WriteLine("Reverse Linked List: " + ListToString(reversed)); // Output: 3 -> 2 -> 1

            // Test MergeTwoLists
            ListNode l1 = new ListNode(1, new ListNode(2, new ListNode(4)));
            ListNode l2 = new ListNode(1, new ListNode(3, new ListNode(4)));
            var merge = new MergeTwoListsSolution();
            ListNode merged = merge.MergeTwoLists(l1, l2);
            Console.WriteLine("Merge Two Lists: " + ListToString(merged)); // Output: 1 -> 1 -> 2 -> 3 -> 4 -> 4
        }

        static string ListToString(ListNode head)
        {
            string result = "";
            while (head != null)
            {
                result += head.val + (head.next != null ? " -> " : "");
                head = head.next;
            }
            return result;
        }
    }
}