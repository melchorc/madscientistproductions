using System;
using System.Windows.Forms;

namespace MadScience
{
    public class ListViewSorter : System.Collections.IComparer
    {

        public void Sort(ListView lstView, ColumnClickEventArgs e)
        {
            lstView.Visible = false;

            lstView.ListViewItemSorter = this;
            if (!(lstView.ListViewItemSorter is ListViewSorter))
                return;
            //this = (ListViewSorter)lstView.ListViewItemSorter;

            if (this.LastSort == e.Column)
            {
                if (lstView.Sorting == SortOrder.Ascending)
                    lstView.Sorting = SortOrder.Descending;
                else
                    lstView.Sorting = SortOrder.Ascending;
            }
            else
            {
                lstView.Sorting = SortOrder.Descending;
            }
            this.ByColumn = e.Column;

            lstView.Sort();
            this.LastSort = e.Column;

            lstView.Visible = true;
        }
        
        public int Compare(object o1, object o2)
        {
            if (!(o1 is ListViewItem))
                return (0);
            if (!(o2 is ListViewItem))
                return (0);

            ListViewItem lvi1 = (ListViewItem)o2;
            string str1 = lvi1.SubItems[ByColumn].Text;
            ListViewItem lvi2 = (ListViewItem)o1;
            string str2 = lvi2.SubItems[ByColumn].Text;

            int result;
            if (lvi1.ListView.Sorting == SortOrder.Ascending)
                result = String.Compare(str1, str2);
            else
                result = String.Compare(str2, str1);

            LastSort = ByColumn;

            return (result);
        }


        private int ByColumn
        {
            get { return Column; }
            set { Column = value; }
        }
        int Column = 0;

        private int LastSort
        {
            get { return LastColumn; }
            set { LastColumn = value; }
        }
        int LastColumn = 0;
    }
}
