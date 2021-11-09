namespace lab2
{
    public class Comment
    {
        public int commentId;
        public string commentText;
        public int userId;
        public int postId;
        public Comment() : base() { }
        public Comment(int commentId, string commentText, int userId, int postId)
        {
            this.commentId = commentId;
            this.commentText = commentText;
            this.postId = postId;
            this.userId = userId;
        }
    }
}