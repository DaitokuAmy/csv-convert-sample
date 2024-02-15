using System;
using UnityEngine;

namespace CsvConvertSample
{
    /// <summary>
    /// セリフ格納用のテーブルデータ
    /// </summary>
    [CreateAssetMenu(menuName = "Csv Convert Sample/Quote Table Data", fileName = "dat_table_quote")]
    public class QuoteTableData : ScriptableObject
    {
        /// <summary>
        /// セリフ情報
        /// </summary>
        [Serializable]
        public class QuoteInfo
        {
            [Tooltip("セリフ内容")]
            public string quote;
            [Tooltip("タグ")]
            public string[] tags;
        }

        [Tooltip("セリフ情報リスト")]
        public QuoteInfo[] quoteInfos;
        [Tooltip("含まれているタグの一覧")]
        public string[] totalTagsProp;
    }
}
