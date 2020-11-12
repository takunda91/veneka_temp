using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndigoFileLoader.objects
{
    public class FileCommentsObject
    {
        public DateTimeOffset FileCommentDate { get; private set; }
        public string Comments { get; private set; }

        public FileCommentsObject(string comments)
        {
            this.FileCommentDate = DateTimeOffset.Now;
            this.Comments = comments;
        }

        public string GetFormatedComment()
        {
            return FileCommentDate.ToString() + " " + Comments;
        }
    }
}
