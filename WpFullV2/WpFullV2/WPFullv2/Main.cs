using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JoeBlogs;

namespace wpPoster
{
    public partial class Main : Form
    {
        //define global variable to access from anywhere
        WordPressWrapper wp;
        string site = "https://kvjdnfkvn.wordpress.com";
        public Main()
        {
           wp = new WordPressWrapper(site+"/xmlrpc.php", "vudane", "t-T123456", 1);

            InitializeComponent();
        }
        public int postWordpress(string title_, string body_, string category_, string keywords_)
        {
            try
            {
                // you should change constructor values to yours info

                // our keywords count can more than 1, so we can add whole keywords to string list
                string[] keyList;
                if (keywords_.Contains(' ')) keyList = keywords_.Split(',');
                else
                {
                    keyList = new string[1];
                    keyList[0] = keywords_;
                }
                // we created new object in Post Class and fill required post fields through this object
                var post = new Post();
                post.DateCreated = DateTime.Now;

                post.Title = title_;
                post.Tags = keyList;
                post.Categories = new string[] { category_ };
                post.Body = body_;
                int index = wp.NewPost(post, true);
                return index; // it will publish post and return post id
            }
            catch (Exception ex)
            {
                // if there is a mistake, the catch block will show message to us
                MessageBox.Show(ex.Message);
            }
            return 0; // if there is a mistake it will return 0
        }
        private void setRecent(int getCount)
        {
            dataGridView1.Rows.Clear();
            // the iterator will get recent posts the desired number
            foreach (var p in wp.GetRecentPosts(getCount))
            {
                dataGridView1.Rows.Add(p.PostID, p.Title, p.Body, p.Categories[0]);
            }
        }
        private void fillPage()
        {
            foreach(var page in wp.GetPages())
            {
                dataGridView2.Rows.Add(page.PageID, page.Title, page.Body, page.DateCreated);
            }
        }
        private void fillAuthors()
        {
            foreach (var author in wp.GetAuthors())
            {
                dataGridView3.Rows.Add(author.UserID, author.LoginName, author.DisplayName);
            }
        }
        private void Main_Load(object sender, EventArgs e)
        {
            
            pSite.Text += site;
            setRecent(20);
            fillPage();
            fillAuthors();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {  // it will call our function and get post id
            int postId = postWordpress(title.Text, body.Text, category.Text, keywords.Text);
            MessageBox.Show("Post published, post id -> "+postId.ToString());
            setRecent(20);
            tabControl1.SelectedIndex = 0;
        }

        private void getRecentB_Click(object sender, EventArgs e)
        {
            setRecent((int)getCountN.Value);
        }

        private void uploadMediaL_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                // it must convert to byte array once
                byte[] imageData = System.IO.File.ReadAllBytes(ofd.FileName);
                FileInfo fi = new FileInfo(ofd.FileName);
                string ad = fi.Name;
                // now we can upload our file through WordpressWrapper object
                var image = wp.NewMediaObject(new MediaObject { Bits = imageData, Name = ad, Type = "image" });
                uploadedPicture.SizeMode = PictureBoxSizeMode.CenterImage;
                // we can get image url and show it
                uploadedPicture.ImageLocation = image.URL;
                uploadedLinkT.Text = image.URL;
            }
        }

        private void keywords_TextChanged(object sender, EventArgs e)
        {
            if (keywords.Text.Contains(","))
            {
                kCount.Text = "keyword count : " + keywords.Text.Split(',').Length.ToString();
            }
            else if (keywords.Text.Length > 0)
                kCount.Text = "keyword count : 1";
        }
    }
}
