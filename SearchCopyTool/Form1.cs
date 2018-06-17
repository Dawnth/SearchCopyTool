using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using System.Security.AccessControl;

namespace SearchCopyTool
{
    public partial class Form1 : Form
    {
        List<string> sSearchKeysTable = new List<string>();
        DirectoryInfo tAllFolder = new DirectoryInfo("C:/");
        // 保存文件夹内及其子文件夹的所有文件名到tFiles
        //FileInfo[] tFiles = new FileInfo[0];
        //Dictionary<string, string> tFiles = new Dictionary<string, string>();
        ArrayList tFiles = new ArrayList();

        public Form1()
        {
            InitializeComponent();

            textBox1.Clear();
            textBox1.Focus();
            textBox3.Text = folderBrowserDialog1.SelectedPath;
            textBox4.Text = folderBrowserDialog2.SelectedPath;

            label1.Text = "";

            dataGridView1.AutoGenerateColumns = false;
        }

        // 关闭时终止所有线程
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            System.Environment.Exit(0);
        }

        // 设置并显示搜索路径
        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
                //Properties.Resources.SearchPath = folderBrowserDialog1.SelectedPath;
            }
        }

        // 设置并显示拷贝路径
        private void button5_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox4.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        // 打开搜索文件夹
        private void button4_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderBrowserDialog1.SelectedPath);
            }
        }

        // 打开拷贝文件夹
        private void button6_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(folderBrowserDialog2.SelectedPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderBrowserDialog2.SelectedPath);
            }
        }

        // 开始搜索
        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            label1.Text = "";
            dataGridView1.Rows.Clear();
            sSearchKeysTable.Clear();
            tFiles.Clear();

            if (getValidSearchKeysTable())
            {
                if (getDirectoryAndFileInfo())
                {
                    textBox2.ForeColor = Color.Red;
                    textBox2.Text = ">>>搜索中，请等待！";

                    // 调用新线程，防止界面假死
                    new Thread((ThreadStart)delegate ()
                    {
                        getSearchResult();
                    }).Start();
                }
                else
                {
                    textBox2.ForeColor = Color.Red;
                    textBox2.Text = ">>>路径无效或无文件，请确认搜索路径！！！";
                }
            }
            else
            {
                textBox2.ForeColor = Color.Red;
                textBox2.Text = ">>>请输入搜索内容！";

                textBox1.Focus();
            }

            // 取消选中
            dataGridView1.ClearSelection();
        }

        // 获得有效的搜索关键字列表
        private bool getValidSearchKeysTable()
        {
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                bool repeatflag = false;
                string tempKey = textBox1.Lines[i].Replace(" ", "").Replace("    ", "").Trim().ToLower();
                if (tempKey != "")
                {
                    foreach (string x in sSearchKeysTable)
                    {
                        if (tempKey == x)
                        {
                            repeatflag = true;
                        }
                    }

                    if (repeatflag == false)
                    {
                        // 添加不重复的查找关键字到数组list
                        sSearchKeysTable.Add(tempKey);
                    }
                    else
                    {
                        repeatflag = false;
                    }
                }
            }

            if (sSearchKeysTable.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 获得有效的搜索文件列表
        private bool getDirectoryAndFileInfo()
        {
            // 检查路径是否为空，检查文件夹是否存在
            if (folderBrowserDialog1.SelectedPath != "" && Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
#if false
                tAllFolder = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                // 保存文件夹内及其子文件夹的所有文件名到tFiles
                tFiles = tAllFolder.GetFiles("*.*", SearchOption.AllDirectories);
#else
                tFiles = GetFile(folderBrowserDialog1.SelectedPath, tFiles);
#endif


                if (tFiles.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static ArrayList GetFile(string path, ArrayList FileList)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            DirectorySecurity s = new DirectorySecurity(path, AccessControlSections.Access);

            //判断目录是否 可以访问  
            if (!s.AreAccessRulesProtected)
            {
                FileInfo[] fil = dir.GetFiles();
                DirectoryInfo[] dii = dir.GetDirectories();
                foreach (FileInfo f in fil)
                {
                    //int size = Convert.ToInt32(f.Length);  
                    long size = f.Length;
                    string[] tempString = new string[3];
                    tempString[0] = f.Extension;
                    tempString[1] = f.FullName;
                    tempString[2] = f.Name;
                    FileList.Add(tempString);//添加文件路径到列表中  
                }
                //获取子文件夹内的文件列表，递归遍历  
                foreach (DirectoryInfo d in dii)
                {
                    GetFile(d.FullName, FileList);
                }
            }
            return FileList;

        }

        // 得到搜索结果并显示
        private void getSearchResult()
        {
            for (int i = 0; i < sSearchKeysTable.Count; i++)
            {
                for (int j = 0; j < tFiles.Count; j++)
                {
                    // 使用arraylist时，由于我们arraylist存储的是数组，所以需要强制类型转换，取出arraylist中的单行元素
                    string[] tempFiles = (string[])tFiles[j];
                    // 添加搜索结果
                    if (tempFiles[2].ToLower().Replace(" ", "").Contains(sSearchKeysTable[i].ToLower()))
                    {
                        // 委托调用
                        dataGridView1.Invoke((MethodInvoker)delegate ()
                        {

                            // 添加到dataGridView
                            DataGridViewRow row = new DataGridViewRow();
                            int index = dataGridView1.Rows.Add(row);
                            dataGridView1.Rows[index].Cells[0].Value = sSearchKeysTable[i];
                            dataGridView1.Rows[index].Cells[1].Value = tempFiles[0];
                            dataGridView1.Rows[index].Cells[2].Value = tempFiles[1];
                            dataGridView1.Rows[index].Cells[3].Value = tempFiles[2];

                            label1.Text = "文件数量: " + this.dataGridView1.RowCount.ToString();

                            // 很重要，不然线程没有休息依然会假死
                            Thread.Sleep(0);
                        });
                    }
                }
            }


            dataGridView1.Invoke((MethodInvoker)delegate ()
            {

                if (dataGridView1.RowCount > 0)
                {
                    textBox2.ForeColor = Color.Green;
                    textBox2.Text = ">>>搜索完成！";

                    //显示在HeaderCell上
                    //数据量大时导致速度过慢假死，暂时注释掉
                    //for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                    //{
                    //    DataGridViewRow r = this.dataGridView1.Rows[i];
                    //    r.HeaderCell.Value = string.Format("{0}", i + 1);
                    //}
                    //this.dataGridView1.Refresh();

                }
                else
                {
                    textBox2.ForeColor = Color.Red;
                    textBox2.Text = ">>>未搜索到相关内容，请尝试更换关键字！";
                }

                // 取消选中
                dataGridView1.ClearSelection();
            });
        }

        // 开始拷贝
        private void button3_Click(object sender, EventArgs e)
        {
            // 检查拷贝目标路径存在
            if (!Directory.Exists(folderBrowserDialog2.SelectedPath))
            {
                Directory.CreateDirectory(folderBrowserDialog2.SelectedPath);
            }

            if (dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    FileInfo fi = new FileInfo(dataGridView1.Rows[i].Cells[2].Value.ToString());
                    fi.CopyTo(folderBrowserDialog2.SelectedPath + "\\" + dataGridView1.Rows[i].Cells[3].Value.ToString(), true);
                }
                textBox2.ForeColor = Color.Green;
                textBox2.Text = ">>>" + dataGridView1.Rows.Count.ToString() + "个文件拷贝完成！" ;
            }
            else
            {
                textBox2.ForeColor = Color.Red;
                textBox2.Text = ">>>未发现需要拷贝的文件！";
            }

        }

        // dataGridView弹出右键菜单
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    {
                        // 要使得DataGridView1.SelectedColumns有返回值,必须做好下述2项准备工作(vb.net2005环境)
                        // 1. 将所有列的sortmode默认属性automatic改为其他非自动排序的选项
                        // 2  要将datagridview1.SelectionMode 属性设置为 DataGridViewSelectionMode.FullColumnSelect 或 DataGridViewSelectionMode.ColumnHeaderSelect
                        // 只有这样能用选定的列填充DataGridView.SelectedColumns 属性.否则DataGridView.SelectedColumns.count的值总是0
                        // 若行已是选中状态就不再进行设置
                        //if (dataGridView1.Rows[e.RowIndex].Selected == false)
                        //{
                        //    //dataGridView1.ClearSelection();
                        //    dataGridView1.Rows[e.RowIndex].Selected = true;
                        //}
                        // 只选中一行时设置活动单元格
                        //if (dataGridView1.SelectedRows.Count == 1)
                        //{
                        //    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        //}
                        
                        // 仅当在选中行时右键才弹出右键菜单
                        for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                        {
                            if (e.RowIndex == dataGridView1.SelectedRows[i].Index)
                            {
                                // 弹出操作菜单
                                contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                textBox2.ForeColor = Color.Red;
                textBox2.Text = ex.Message;
            }
        }

        // 更改dataGridView时，更新文件数量
        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            label1.Text = "文件数量: " + this.dataGridView1.Rows.Count.ToString();
        }

        // 右键删除
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            for (int i = dataGridView1.SelectedRows.Count; i > 0; i--)
            {
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[i - 1]);
            }

            // 取消选中
            dataGridView1.ClearSelection();
        }

        // 右键拷贝
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // 检查拷贝目标路径存在
            if (!Directory.Exists(folderBrowserDialog2.SelectedPath))
            {
                Directory.CreateDirectory(folderBrowserDialog2.SelectedPath);
            }

            if (dataGridView1.SelectedRows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    FileInfo fi = new FileInfo(dataGridView1.Rows[dataGridView1.SelectedRows[i].Index].Cells[2].Value.ToString());
                    fi.CopyTo(folderBrowserDialog2.SelectedPath + "\\" + dataGridView1.Rows[dataGridView1.SelectedRows[i].Index].Cells[3].Value.ToString(), true);
                }
                textBox2.ForeColor = Color.Green;
                textBox2.Text = ">>>" + dataGridView1.SelectedRows.Count.ToString() + "个文件拷贝完成！";
            }
            else
            {
                textBox2.ForeColor = Color.Red;
                textBox2.Text = ">>>未发现需要拷贝的文件！";
            }
        }
    }
}