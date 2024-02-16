using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CsvConvertSample.Editor
{
    /// <summary>
    /// Csv変換用のウィザードウィンドウ
    /// </summary>
    public class CsvConverter : EditorWindow
    {
        /// <summary>
        /// CSVの1行あたりの情報（CSVの中身をここに展開する）
        /// </summary>
        [Serializable]
        private class Record
        {
            public string quote;
            public string tag1;
            public string tag2;
            public string tag3;
            public string tag4;
            public string tag5;
            public string tag6;
            public string tag7;
            public string tag8;
        }

        [SerializeField] private TextAsset[] _sourceCsvAssets;
        [SerializeField] private QuoteTableData _destinationAsset;

        /// <summary>
        /// ウィザードを開く処理
        /// </summary>
        [MenuItem("Tools/Csv Converter")]
        private static void Open()
        {
            // ウィンドウを開く（既に開いている場合はアクティブ化のみ）
            var window = GetWindow<CsvConverter>();

            // タイトル表示の修正
            // NicifyVariableNameはUnityEditorでよく出てくる英語表記に合わせて文字を変換してくれる物（スペース入れたり）
            // アイコン名一覧：https://github.com/halak/unity-editor-icons
            window.titleContent = new GUIContent(ObjectNames.NicifyVariableName(nameof(CsvConverter)),
                EditorGUIUtility.FindTexture("d_CustomTool"));
        }

        /// <summary>
        /// GUIの描画
        /// </summary>
        private void OnGUI()
        {
            // 自身のクラスに定義されたSerializeFieldを編集するための前準備
            var serializedObject = new SerializedObject(this);
            serializedObject.Update();

            //---- ココがGUIの構築 ----//
            // 読み込み元CSVリストの入力フィールド描画
            var sourceCsvAssets = serializedObject.FindProperty(nameof(_sourceCsvAssets));
            EditorGUILayout.PropertyField(sourceCsvAssets, new GUIContent("読み込み元のCSVファイルリスト"), true);

            // 出力するTableDataの読み込み
            var destinationAsset = serializedObject.FindProperty(nameof(_destinationAsset));
            EditorGUILayout.PropertyField(destinationAsset, new GUIContent("出力先のセリフテーブルデータ"));

            // 編集したSerializeFieldをメモリに反映させる
            serializedObject.ApplyModifiedProperties();

            // コンバートボタン
            if (GUILayout.Button("変換"))
            {
                Convert();
            }
        }

        /// <summary>
        /// 変換処理
        /// </summary>
        private void Convert()
        {
            try
            {
                //---- ここが読み込み（Import）部分 ----//
                // CSVファイルからデータを抽出
                var totalRecords = new List<Record>();
                foreach (var csvAsset in _sourceCsvAssets)
                {
                    // 入力が空ならSkip
                    if (csvAsset == null)
                    {
                        continue;
                    }

                    // CSVからデータ構造を読み込んでTotalRecordsに合成
                    var records = CSVSerializer.Deserialize<Record>(csvAsset.text);
                    totalRecords.AddRange(records);
                }

                //---- ここが出力（Export）部分 ----//
                // QuoteTableDataを編集する準備
                var serializedObj = new SerializedObject(_destinationAsset);
                serializedObj.Update();

                // Record情報を入れる
                var totalTags = new List<string>();
                var quoteInfosProp = serializedObj.FindProperty("quoteInfos");
                quoteInfosProp.arraySize = totalRecords.Count;
                for (var i = 0; i < totalRecords.Count; i++)
                {
                    var record = totalRecords[i];

                    // セリフ文字列の格納
                    var quoteInfoProp = quoteInfosProp.GetArrayElementAtIndex(i);
                    quoteInfoProp.FindPropertyRelative("quote").stringValue = record.quote;

                    // 含まれるTagを配列として格納
                    var tags = GetTags(record);
                    var tagsProp = quoteInfoProp.FindPropertyRelative("tags");
                    tagsProp.arraySize = tags.Length;
                    for (var j = 0; j < tags.Length; j++)
                    {
                        tagsProp.GetArrayElementAtIndex(j).stringValue = tags[j];
                    }

                    // Tag一覧を作るためにリストに追加
                    totalTags.AddRange(tags);
                }

                // 含まれるタグを列挙して格納
                var totalTagsProp = serializedObj.FindProperty("totalTags");
                totalTags = totalTags.Distinct().ToList(); // Distinctによって重複のタグを取り除く
                totalTagsProp.arraySize = totalTags.Count;
                for (var i = 0; i < totalTags.Count; i++)
                {
                    totalTagsProp.GetArrayElementAtIndex(i).stringValue = totalTags[i];
                }

                // QuoteTableDataに変更を反映
                serializedObj.ApplyModifiedPropertiesWithoutUndo();

                Debug.Log("Convert Completed.");
            }
            catch (Exception exception)
            {
                // エラーになったらここに来るのでエラー内容を出力する
                Debug.LogException(exception);
                Debug.LogError("Convert Failed.");
            }
        }

        /// <summary>
        /// Recordに入っているTagを配列として取得する
        /// </summary>
        private string[] GetTags(Record record)
        {
            return new[]
            {
                record.tag1,
                record.tag2,
                record.tag3,
                record.tag4,
                record.tag5,
                record.tag6,
                record.tag7,
                record.tag8,
            }.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }
    }
}