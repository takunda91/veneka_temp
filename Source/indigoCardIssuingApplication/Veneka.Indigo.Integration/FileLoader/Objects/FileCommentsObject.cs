using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    public sealed class FileCommentsObject
    {
        public delegate void LogMessage(object message);
        public delegate void LogException(object message, Exception ex);

        public DateTimeOffset FileCommentDate { get; private set; }
        public string Comments { get; private set; }
        public Exception FileCommentException { get; private set; }

        public FileCommentsObject(string comments)
        {
            this.FileCommentDate = DateTimeOffset.Now;
            this.Comments = comments;
        }

        public FileCommentsObject(string comments, LogMessage logMessage)
        {
            this.FileCommentDate = DateTimeOffset.Now;
            this.Comments = comments;
            logMessage(comments);
        }

        public FileCommentsObject(string comments, Exception ex, LogException logException)
        {
            this.FileCommentDate = DateTimeOffset.Now;
            this.Comments = comments;
            this.FileCommentException = ex;
            logException(comments, ex);
        }

        public string GetFormatedComment()
        {
            if(FileCommentException == null)
                return String.Format("{0} {1}", FileCommentDate.ToString(), Comments);
            else
                return String.Format("{0} {1} {2}", FileCommentDate.ToString(), Comments, FileCommentException.Message);
        }
    }
}
