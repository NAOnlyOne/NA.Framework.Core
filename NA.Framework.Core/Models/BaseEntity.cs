using NA.Framework.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Models
{
    public class BaseEntity<TPK>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public virtual TPK Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now.ToCNZone();

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now.ToCNZone();

        /// <summary>
        /// 软删除标志
        /// </summary>
        public int IsDeleted { get; set; }
    }
}
