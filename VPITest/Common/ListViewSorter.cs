using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace VPITest.Common
{
    public class ListViewSorter : IComparer
    {
        private int columnToSort;
        private SortOrder orderOfSort;
        private CaseInsensitiveComparer objectCompare;
        public ListViewSorter()
        {
            columnToSort = 0;
            orderOfSort = SortOrder.None;
            objectCompare = new CaseInsensitiveComparer();
        }

        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listViewX, listViewY;
            listViewX = (ListViewItem)x;
            listViewY = (ListViewItem)y;
            compareResult = objectCompare.Compare(listViewX.SubItems[columnToSort].Text,
                listViewY.SubItems[columnToSort].Text);
            if (orderOfSort == System.Windows.Forms.SortOrder.Ascending)
            {
                return compareResult;
            }
            else if(orderOfSort == System.Windows.Forms.SortOrder.Descending)
            {
                return (-compareResult);
            }
            else 
            {
                return 0;
            }
        }

        public int SortColumn
        {
            get { return columnToSort; }
            set { columnToSort = value; }
        }

        public SortOrder OrderOfSort
        {
            get { return orderOfSort; }
            set { orderOfSort = value; }
        }
    }
}
