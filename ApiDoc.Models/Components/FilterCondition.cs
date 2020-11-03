using System;
using System.Collections.Generic;
using System.Text;

namespace ApiDoc.Models.Components
{
    //组合查询条件
    public class FilterCondition
    { 
        public string BracketL { get; set; }    // (
        public string ColumnName { get; set; }  //数据列名  
        public string ValueType { get; set; }   //数据类型:List String DateTime Decimal Int32 Booleanbool

        /// <summary>
        /// List: NotContain(not in), NotEqual(not in), Contain(in)
        /// String: Equal(=), Contain(like '%'), NotEqual(<>), MatchLeft(like '%), MatchRight, NotContain(not like '%), 其它 like '%'
        /// Decimal Int32: Equal(=), NotEqual(<>),  Greater(>)， Less(<)
        /// DateTime: Equal(= ), Greater(>), Less(<)
        /// Boolean: Equal
        /// </summary>
        public ConditionTypeEnum Condition { get; set; }   //比较条件 
        public string Value { get; set; }       //值
        public string BracketR { get; set; }    // )
        public string JoinType { get; set; }    //And 或 Or

    }

    /// <summary>
    /// 比较条件类型
    /// </summary>
    public enum ConditionTypeEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 大于
        /// </summary>
        Greater,
        /// <summary>
        /// 小于
        /// </summary>
        Less,
        /// <summary>
        /// 包含
        /// </summary>
        Contain,
        /// <summary>
        /// 前匹配
        /// </summary>
        MatchLeft,
        /// <summary>
        /// 后匹配
        /// </summary>
        MatchRight,
        /// <summary>
        /// 不等于
        /// </summary>
        NotEqual,
        /// <summary>
        /// 不包含
        /// </summary>
        NotContain,
        /// <summary>
        /// 未知
        /// </summary>
        Unknown
    }
}
