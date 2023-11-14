using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class PageNumberNavigator
    {
        private int pageSize;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }
        

        private int navSize;
        public int NavSize
        {
            get { return navSize; }
            set { navSize = value; }
        }


        private string pageUrl;
        public string PageUrl
        {
            get { return pageUrl; }
            set { pageUrl = value; }
        }


        private int dataCount;
        public int DataCount
        {
            get { return dataCount; }
            set 
            { 
                dataCount = value;

                float f = (float)dataCount / pageSize;

                if (f - (int)f == 0) { lastPage = (int)f; }
                else { lastPage = (int)f + 1; }
            }
        }


        private int lastPage;
        public int LastPage
        {
            get { return lastPage; }
            set { lastPage = value; }
        }


        private int currentPage;
        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; }
        }


        private string currentPageNumAsString;
        public string CurrentPageNumAsString
        {
            get { return currentPageNumAsString; }
            set {
                currentPageNumAsString = value;
                if (value == null)
                {
                    currentPage = 1;
                }
                else
                {
                    currentPage = int.Parse(value);
                }
            }
        }


        public PageNumberNavigator()
        {
            
        }
        public virtual string InsertNavigator()
        {
            string navigator = "";
            int half = (int)(navSize / 2);
            int start, stop;
            bool etcAtStart, etcAtStop;




            if (currentPage - half < 0)
            {
                start = 1;
                etcAtStart = false;
            }
            else if (currentPage <= navSize)
            {
                start = 1;
                etcAtStart = false;
            }
            else
            {
                start = currentPage - half;
                etcAtStart = true;
            }




            if (currentPage + half > lastPage)
            {
                stop = lastPage;
                if (etcAtStart)
                {
                    start = lastPage - navSize + 1;
                }
                etcAtStop = false;
            }
            else if (lastPage - currentPage < navSize)
            {
                stop = lastPage;
                etcAtStop = false;
            }
            else
            {
                if (!etcAtStart)
                {
                    stop = navSize;
                    if (stop == currentPage)
                    {
                        stop++;
                    }
                }
                else
                {
                    stop = currentPage + half;
                }
                etcAtStop = true;
            }





            string seperator = "?";
            if (pageUrl.Split('/').Last().Contains('?'))
            {
                seperator = "&&";
            }

            if (etcAtStart)
            {
                navigator += $"<a href='{pageUrl}{seperator}p={1}'>{1}</a>";
                navigator += "<h5 style='display:inline;'> . . . </h5>";
            }

            for (int i = start; i <= stop; i++)
            {
                if (lastPage == 1) { continue; }

                if (i == currentPage)
                {
                    navigator += $"<a href='{pageUrl}{seperator}p={i}' class='active'>{i}</a>";
                }
                else
                {
                    navigator += $"<a href='{pageUrl}{seperator}p={i}'>{i}</a>";
                }
            }
            if (etcAtStop)
            {
                navigator += "<h5 style='display:inline;'> . . . </h5>";
                navigator += $"<a href='{pageUrl}{seperator}p={lastPage}'>{lastPage}</a>";
            }
            return navigator;
        }
    }
}
