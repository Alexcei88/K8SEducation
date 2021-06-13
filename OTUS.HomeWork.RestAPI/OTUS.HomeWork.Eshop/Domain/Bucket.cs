using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OTUS.HomeWork.EShop.Domain
{
    public record Bucket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 

        public Guid UserId { get; set; }

        public SortedSet<BucketItem> Items { get; set; } = new SortedSet<BucketItem>();
    }

    [Table("bucket_item")]
    public record BucketItem
        : IComparable
    {
        [Key]
        public Guid ProductId { get; init; }

        [Key]
        public Guid BucketId { get;set;}

        public Bucket Bucket { get; set; }

        public int Quantity { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is BucketItem)
                return -1;

            BucketItem bucketProduct = obj as BucketItem;
            return bucketProduct.ProductId.CompareTo(this.ProductId);
        }
    }
}
