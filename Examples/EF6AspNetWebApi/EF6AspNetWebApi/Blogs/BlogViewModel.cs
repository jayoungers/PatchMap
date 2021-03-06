﻿using EF6AspNetWebApi.Blogs.Posts;
using EF6AspNetWebApi.Data;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EF6AspNetWebApi.Blogs
{
    public class BlogSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public static Expression<Func<Blog, BlogSummaryViewModel>> Map()
        {
            return i => new BlogSummaryViewModel
            {
                Id = i.BlogId,
                Name = i.Name,
                Url = i.Url
            };
        }
    }

    public class BlogViewModel : BlogSummaryViewModel
    {
        public List<string> Tags { get; set; } = new List<string>();
        public List<PostSummaryViewModel> Posts { get; set; } = new List<PostSummaryViewModel>();
        public PostSummaryViewModel PromotedPost { get; set; }

        public new static Expression<Func<Blog, BlogViewModel>> Map()
        {
            var postMapper = PostSummaryViewModel.Map();

            return i => new BlogViewModel
            {
                Id = i.BlogId,
                Name = i.Name,
                Url = i.Url,
                Tags = i.Tags.OrderBy(t => t.Name).Select(t => t.Name).ToList(),
                Posts = i.Posts.OrderByDescending(p => p.DateCreated).Select(p => postMapper.Invoke(p)).ToList(),
                PromotedPost = i.PromotedPostId == null ? null : postMapper.Invoke(i.PromotedPost)
            };
        }
    }
}
