using System;
using System.Windows.Forms;

namespace MadScience
{
    public class ListViewSorter : System.Collections.IComparer
    {

        public void Sort(ListView lstView, ColumnClickEventArgs e)
        {
            lstView.Visible = false;

			if (!(lstView.ListViewItemSorter is ListViewSorter))
			{
				lstView.ListViewItemSorter = this;
			}
            //lstView.ListViewItemSorter = this;
            //if (!(lstView.ListViewItemSorter is ListViewSorter))
                //return;
            //this = (ListViewSorter)lstView.ListViewItemSorter;

            if (this.LastColumn == e.Column)
            {
                if (lstView.Sorting == SortOrder.Ascending)
                    lstView.Sorting = SortOrder.Descending;
                else
                    lstView.Sorting = SortOrder.Ascending;
            }
            else
            {
                lstView.Sorting = SortOrder.Ascending;
            }

            //this.ByColumn = e.Column;
			this.LastColumn = e.Column;

            lstView.Sort();
            

            lstView.Visible = true;
        }

		public Boolean IsNumeric(Object Expression)
		{
			if (Expression == null || Expression is DateTime) return false;

			if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean) return true;

			try
			{
				//if (Expression is string)
				//{
				//	Double.Parse(Expression as string);
				//}
				//else
				//{
					Double.Parse(Expression.ToString());
					return true;
				//}
			}
			catch { } // just dismiss errors but return false
			return false;
		}

        public int Compare(object o1, object o2)
        {
			if (this.LastColumn == -1) return 0;

            if (!(o1 is ListViewItem))
                return (0);
            if (!(o2 is ListViewItem))
                return (0);

            ListViewItem lvi1 = (ListViewItem)o2;
            string str1 = lvi1.SubItems[LastColumn].Text;
            ListViewItem lvi2 = (ListViewItem)o1;
			string str2 = lvi2.SubItems[LastColumn].Text;

			int result = 0;

			if (lvi1.ListView.Columns[LastColumn].TextAlign == HorizontalAlignment.Right)
			{
				double dbl1 = Double.Parse(str1);
				double dbl2 = Double.Parse(str2);

				// Numeric compare
				if (lvi1.ListView.Sorting == SortOrder.Ascending)
				{
					if (dbl1 == dbl2) result = 0;
					if (dbl1 < dbl2) result = -1;
					if (dbl2 > dbl1) result = 1;
				}
				else
				{
					if (dbl1 == dbl2) result = 0;
					if (dbl2 < dbl1) result = 1;
					if (dbl2 > dbl1) result = -1;
				}
			}

			if (lvi1.ListView.Columns[LastColumn].TextAlign == HorizontalAlignment.Left)
			{

				if (lvi1.ListView.Sorting == SortOrder.Ascending)
				{
					if (lvi1.ListView.Columns[LastColumn].Tag != null && lvi1.ListView.Columns[LastColumn].Tag.ToString() == "DateTime")
					{
						DateTime date1 = Convert.ToDateTime(str1);
						DateTime date2 = Convert.ToDateTime(str2);
						TimeSpan t1 = new TimeSpan();
						t1 = date1 - date2;
						if (t1.Seconds == 0) result = 0;
						if (t1.Seconds < 0) result = 1;
						if (t1.Seconds > 0) result = -1;
						
					}
					else
					{
						result = String.Compare(str1, str2);
					}
				}
				else
				{
					if (lvi1.ListView.Columns[LastColumn].Tag != null && lvi1.ListView.Columns[LastColumn].Tag.ToString() == "DateTime")
					{
						DateTime date1 = Convert.ToDateTime(str2);
						DateTime date2 = Convert.ToDateTime(str1);
						TimeSpan t1 = new TimeSpan();
						t1 = date1 - date2;
						if (t1.Seconds == 0) result = 0;
						if (t1.Seconds < 0) result = 1;
						if (t1.Seconds > 0) result = -1;

					}
					else
					{
						result = String.Compare(str2, str1);
					}
				}
			}

            //LastSort = ByColumn;

            return (result);
        }


        private int ByColumn
        {
            get { return Column; }
            set { Column = value; }
        }
        private int Column = 0;

        //private int LastSort
        //{
            //get { return LastColumn; }
            //set { LastColumn = value; }
        //}

        private int LastColumn = -1;
    }
}
