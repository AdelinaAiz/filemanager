using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace Файловый_менеджер
{
    public partial class Form1 : Form
    {

     //   private List<Button> LeftLogicalDrives = new List<Button>();
     //   private List<Button> RightLogicalDrives = new List<Button>();

        private string LeftViewRootPath = "C:\\";
        private string RightViewRootPath = "C:\\";

        private FileSystemWatcher LeftViewObserver = new FileSystemWatcher();
        private FileSystemWatcher RightViewObserver = new FileSystemWatcher();

        private bool isLeftActive = true;

        public Form1()
        {
            InitializeComponent();
        }


            private void listView2_SelectedIndexChanged(object sender, EventArgs e) { }

            private void Form1_Load(object sender, EventArgs e)
            {
                this.DoubleBuffered = true;
                UpdateListView(LeftListView, LeftViewRootPath);
                TextBox.Text = LeftViewRootPath;
                UpdateListView(RightListView, RightViewRootPath);
                //           LeftListView.Width = this.Size.Width/2 - 5;
                //           RightListView.Location = new Point(this.Size.Width/2 , 70);
                this.Text = "Файловый менеджер";
            }

            private void UpdateListView(ListView listView, string root)
            {
                try
                {
                    listView.Items.Clear();
                    DirectoryInfo dirInfo = new DirectoryInfo(root);
                    var files = dirInfo.GetFiles();
                    var folders = dirInfo.GetDirectories();

                    if (dirInfo.Parent != null)
                        listView.Items.Add(BuildListViewItem(
                            "...",
                            "",
                            "",
                            ""
                        ));

                    foreach (DirectoryInfo folder in folders)
                    {
                        ListViewItem item = BuildListViewItem(
                            System.IO.Path.GetFileName(folder.FullName), //получаем путь к файлу и получаем имя
                            "",
                            "<папка>",
                            folder.CreationTime.ToShortDateString() + " " + folder.CreationTime.ToShortTimeString()
                        );
                        item.ImageIndex = 0;
                        listView.Items.Add(item);
                    }

                    foreach (FileInfo file in files)
                    {
                        string ext;
                        if (file.Extension.Length > 0)
                            ext = file.Extension.Substring(1);
                        else
                            ext = "";

                        listView.Items.Add(BuildListViewItem(
                        System.IO.Path.GetFileNameWithoutExtension(file.FullName),
                        ext,
                        GetSize(file.Length),
                        file.CreationTime.ToShortDateString() + " " + file.CreationTime.ToShortTimeString()
                        ));
                    }
                }
                catch (IOException)
                {
                    MessageBox.Show("Устройство не готово!");
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Отказано в доступе!");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace);
                }

            }

            //Построение объекта ListViewItem для ListView в программе
            private ListViewItem BuildListViewItem(string Name, string type, string size, string date)
            {
                ListViewItem item = new ListViewItem(Name); //ListViewItem - то что отображается на ListView
                item.SubItems.Add(type);
                item.SubItems.Add(size);
                item.SubItems.Add(date);
                return item;
            }

            // Форматируем размер файла из числа байт в необходимые единицы измерения
            private string GetSize(long size)
            {
                double bytes = size;
                double kBytes = Math.Round(bytes / 1024, 1);
                double mBytes = Math.Round(kBytes / 1024, 1);
                double gBytes = Math.Round(mBytes / 1024, 1);

                if (gBytes >= 1)
                    return String.Format("{0} Гб", gBytes);
                else if (mBytes >= 1)
                    return String.Format("{0} Мб", mBytes);
                else if (kBytes >= 1)
                    return String.Format("{0} Кб", kBytes);
                else return String.Format("{0} байт", bytes);
            }

            private void LeftListView_SelectedIndexChanged(object sender, EventArgs e)
            {
                isLeftActive = true;
                TextBox.Text = LeftViewRootPath;
            }

            private void RightListView_SelectedIndexChanged(object sender, EventArgs e)
            {
                isLeftActive = false;
                TextBox.Text = RightViewRootPath;
            }

            private void button1_Click(object sender, EventArgs e)
            {
                if (isLeftActive == false)
                {
                    if (Directory.Exists(TextBox.Text))
                    {
                        RightViewObserver.Path = RightViewRootPath;
                        RightViewRootPath = TextBox.Text;
                        UpdateListView(RightListView, RightViewRootPath);
                    }
                    else
                    {
                        MessageBox.Show("Неверный путь!");
                        TextBox.Text = RightViewRootPath;
                    }
                }
                else
                {
                    if (Directory.Exists(TextBox.Text)) {
                        LeftViewObserver.Path = LeftViewRootPath;
                        LeftViewRootPath = TextBox.Text;
                        UpdateListView(LeftListView, LeftViewRootPath);
                    }
                    else
                    {
                        MessageBox.Show("Неверный путь!");
                        TextBox.Text = LeftViewRootPath;
                    }
                }
            }

            private void LeftListView_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                //если выбранный элемент не является папкой или переходом к родительскому каталогу, выходим
                if (!LeftListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>") && !LeftListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    if (LeftListView.SelectedItems[0].SubItems[1].Text != "sys")
                    {
                        string p = TextBox.Text + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                        TextBox.Text = p;
                        //           string path = "C:\\Games\\BERS.jpg";
                        FileInfo file = new FileInfo(p);
                        Process.Start(p);
                    }
                    //Process.Start(Path.Combine(TextBox.Text, LeftListView.SelectedItems[0].SubItems[2].Text));
                    return;
                }

                //если выбранный элемент является переходом к родительскому каталогу
                if (LeftListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    try
                    {
                        string parent = Directory.GetParent(LeftViewRootPath).FullName;
                        LeftViewObserver.Path = parent;
                        LeftViewRootPath = parent;
                        TextBox.Text = parent;
                        UpdateListView(LeftListView, LeftViewRootPath);
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("Отказано в доступе!");
                    }
                    return;
                }

                try
                {
                    string child = LeftViewRootPath;
                    if (LeftViewRootPath[LeftViewRootPath.Length - 1] != '\\') child += "\\";
                    child += LeftListView.SelectedItems[0].SubItems[0].Text;
                    LeftViewObserver.Path = child;
                    LeftViewRootPath = child;
                    TextBox.Text = child;
                    UpdateListView(LeftListView, LeftViewRootPath);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Отказано в доступе!");
                }
            }

            private void RightListView_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                //если выбранный элемент не является папкой или переходом к родительскому каталогу, выходим
                if (!RightListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>") && !RightListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    if (RightListView.SelectedItems[0].SubItems[1].Text != "sys")
                    {
                        string p = TextBox.Text + "\\" + RightListView.SelectedItems[0].SubItems[0].Text + '.' + RightListView.SelectedItems[0].SubItems[1].Text;
                        TextBox.Text = p;
                        //           string path = "C:\\Games\\BERS.jpg";
                        FileInfo file = new FileInfo(p);
                        Process.Start(p);
                    }
                    //Process.Start(Path.Combine(TextBox.Text, LeftListView.SelectedItems[0].SubItems[2].Text));
                    return;
                }

                //если выбранный элемент является переходом к родительскому каталогу
                if (RightListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    try
                    {
                        string parent = Directory.GetParent(RightViewRootPath).FullName;
                        RightViewObserver.Path = parent;
                        RightViewRootPath = parent;
                        TextBox.Text = parent;
                        UpdateListView(RightListView, RightViewRootPath);
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("Отказано в доступе!");
                    }
                    return;
                }
                try
                {
                    string child = RightViewRootPath;
                    if (RightViewRootPath[RightViewRootPath.Length - 1] != '\\') child += "\\";
                    child += RightListView.SelectedItems[0].SubItems[0].Text;
                    RightViewObserver.Path = child;
                    RightViewRootPath = child;
                    TextBox.Text = child;
                    UpdateListView(RightListView, RightViewRootPath);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Отказано в доступе!");
                }
            }

            private void button2_Click(object sender, EventArgs e){delete();}

            

            private void LeftListView_MouseUp(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(MousePosition, ToolStripDropDownDirection.Right);
                }
            }

            private void RightListView_MouseUp(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(MousePosition, ToolStripDropDownDirection.Right);
                }
            }

            private void удалитьToolStripMenuItem_Click(object sender, EventArgs e){delete();}


        private void создатьПапкуToolStripMenuItem_Click(object sender, EventArgs e){create();}

        private void button3_Click(object sender, EventArgs e){copy();}


        void delete()
        {
            DialogResult dialogResult = MessageBox.Show("Удалить выбранный элемент?", "Удаление", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string p;
                if (isLeftActive == false)
                {
                    p = TextBox.Text + "\\" + RightListView.SelectedItems[0].SubItems[0].Text + '.' + RightListView.SelectedItems[0].SubItems[1].Text;
                    TextBox.Text = p;

                    if (RightListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>"))
                    {
                        string deletePath = p; //Можно написать вот так string deletePath = @"D:\FolderToDelete";
                        deleteFolder(deletePath); 
                        try
                        {
                            Directory.Delete(deletePath);
                        }
                        catch
                        {
                            MessageBox.Show("При удалении папки возникли ошибки");
                        }
                    }
                    else if (RightListView.SelectedItems[0].SubItems[1].Text != "")
                    {
                        FileInfo file = new FileInfo(p);
                        file.Delete();
                    }
                    UpdateListView(LeftListView, LeftViewRootPath);
                    UpdateListView(RightListView, RightViewRootPath);
                    MessageBox.Show("Удалено успешно");
                }
                else
                {
                    p = TextBox.Text + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                    TextBox.Text = p;
                    if (LeftListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>"))
                    {
                        string deletePath = p; //Можно написать вот так string deletePath = @"D:\FolderToDelete";
                        deleteFolder(deletePath); 
                        try
                        {
                            Directory.Delete(deletePath);
                            //    MessageBox.Show("Папка {0} успешно удалена", deletePath);
                        }
                        catch
                        {
                            MessageBox.Show("При удалении папки возникли ошибки");
                        }
                    }
                    else if (LeftListView.SelectedItems[0].SubItems[1].Text != "")
                    {
                        FileInfo file = new FileInfo(p);
                        file.Delete();
                    }
                    UpdateListView(LeftListView, LeftViewRootPath);
                    UpdateListView(RightListView, RightViewRootPath);
                    MessageBox.Show("Удалено успешно");
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("Удаление прервано");
            }
        }

        static void deleteFolder(string folder)
        {
            try
            {
                //Класс DirectoryInfo работает с папками
                DirectoryInfo di = new DirectoryInfo(folder);
                //Создаём массив дочек папок
                DirectoryInfo[] diA = di.GetDirectories();
                //Создаём массив дочек файлов
                FileInfo[] fi = di.GetFiles();
                //удалим файлы в папке
                foreach (FileInfo f in fi)
                {
                    f.Delete();
                }
                //удалим папки
                foreach (DirectoryInfo df in diA)
                {
                    //Как раз пошла рекурсия
                    deleteFolder(df.FullName);
                    //Если в папке нет больше вложенных папок и файлов - удаляем её,
                    if (df.GetDirectories().Length == 0 && df.GetFiles().Length == 0) df.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка.  Ошибка: " + ex.Message);
            }
        }

        void copy()
        {
            DialogResult dialogResult = MessageBox.Show("Копировать выбранный элемент?", "Удаление", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string p1, p2;
                if (isLeftActive == true)
                {
                    if (!LeftListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>") && !LeftListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                    {
                        try
                        {
                            p1 = TextBox.Text + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                            p2 = RightViewRootPath + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                            TextBox.Text = p1;
                            File.Copy(p1, p2, true);
                            UpdateListView(RightListView, RightViewRootPath);
                            MessageBox.Show("Копировано успешно");
                        }
                        catch
                        {
                            MessageBox.Show("При копировании возникли ошибки");
                        }
                    }
                    if (LeftListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>"))
                    {
                        try
                        {
                            p1 = TextBox.Text + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text;
                            p2 = RightViewRootPath + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text;
                            copyFolder(p1, p2);
                            UpdateListView(RightListView, RightViewRootPath);
                            MessageBox.Show("Копировано успешно");
                        }
                        catch
                        {
                            MessageBox.Show("При копировании возникли ошибки");
                        }
                    }
                }
                else if (isLeftActive == false)
                {
                    if (!RightListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>") && !RightListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                    {
                        try
                        {
                            p1 = TextBox.Text + "\\" + RightListView.SelectedItems[0].SubItems[0].Text + '.' + RightListView.SelectedItems[0].SubItems[1].Text;
                            p2 = LeftViewRootPath + "\\" + RightListView.SelectedItems[0].SubItems[0].Text + '.' + RightListView.SelectedItems[0].SubItems[1].Text;
                            TextBox.Text = p1;
                            File.Copy(p1, p2, true);
                            UpdateListView(LeftListView, LeftViewRootPath);
                            MessageBox.Show("Копировано успешно");
                        }
                        catch
                        {
                            MessageBox.Show("При копировании возникли ошибки");
                        }
                    }
                    if (RightListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>"))
                    {
                        try
                        {
                            p1 = TextBox.Text + "\\" + RightListView.SelectedItems[0].SubItems[0].Text;
                            p2 = LeftViewRootPath + "\\" + RightListView.SelectedItems[0].SubItems[0].Text;
                            copyFolder(p1, p2);
                            UpdateListView(LeftListView, LeftViewRootPath);
                            MessageBox.Show("Копировано успешно");
                        }
                        catch
                        {
                            MessageBox.Show("При копировании возникли ошибки");
                        }
                    }
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("Копирование прервано");
            }
        }

        void copyFolder(string p1, string p2)//путь папки с названием 
        {
            if (!Directory.Exists(p2))
            {
                Directory.CreateDirectory(p2);
            }
            string[] fi = Directory.GetFiles(p1);
            foreach (string f in fi)
                File.Copy(f, Path.Combine(p2, Path.GetFileName(f)));
            string[] fo = Directory.GetDirectories(p1);
            foreach (string f in fo)
            {
                copyFolder(f, Path.Combine(p2, Path.GetFileName(f)));
            }
        }

        void create()
        {
            string p;
            p = TextBox.Text + "\\" + "Новая папка";
            int i = 1;
            if (Directory.Exists(p))
            {
                p = TextBox.Text + "\\" + "Новая папка(" + i + ")";
                while (Directory.Exists(p))
                {
                    i++;
                    p = TextBox.Text + "\\" + "Новая папка(" + i + ")";
                }
            }
            Directory.CreateDirectory(p);
            UpdateListView(RightListView, RightViewRootPath);
            UpdateListView(LeftListView, LeftViewRootPath);
        }

        private void panel2_Paint(object sender, PaintEventArgs e){}

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e){copy();}

        private void button5_Click(object sender, EventArgs e) {create();}

        private void RightListView_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))//перетаскивается ли файл
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void RightListView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))//перетаскивается ли файл
            {
                string p;
                 string[] s = (string[])e.Data.GetData(DataFormats.FileDrop);
                 foreach(string fn in s)
                {
                    p = RightViewRootPath;
                    if (System.IO.File.Exists(fn))
                    {
                        File.Copy(fn, Path.Combine(p, Path.GetFileName(fn)));
                    }
                    else
                    {
                        copyFolder(fn, Path.Combine(p, Path.GetFileName(fn)));
                    }
                    UpdateListView(RightListView, RightViewRootPath);
                }

            }
        }

        private void LeftListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))//перетаскивается ли файл
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void LeftListView_DragLeave(object sender, EventArgs e)
        {

        }

        private void LeftListView_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))//перетаскивается ли файл
            {
                string p;
                string[] s = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string fn in s)
                {
                    p = LeftViewRootPath;
                    File.Copy(fn, Path.Combine(p, Path.GetFileName(fn)));
                    UpdateListView(LeftListView, RightViewRootPath);
                    /*
                        string p1, p2;
                        p1 = TextBox.Text + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                        p2 = RightViewRootPath + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                        TextBox.Text = p1;
                        File.Copy(p1, p2, true);
                        UpdateListView(RightListView, RightViewRootPath);
                        */
                }
            }
        }

        private void LeftListView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Z)           
                copy();
            if (e.KeyCode == Keys.X)
                delete();
            if (e.KeyCode == Keys.C)
                create();
            if(e.KeyCode == Keys.Enter)
            {
                //если выбранный элемент не является папкой или переходом к родительскому каталогу, выходим
                if (!LeftListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>") && !LeftListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    if (LeftListView.SelectedItems[0].SubItems[1].Text != "sys")
                    {
                        string p = TextBox.Text + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                        TextBox.Text = p;
                        //           string path = "C:\\Games\\BERS.jpg";
                        FileInfo file = new FileInfo(p);
                        Process.Start(p);
                    }
                    //Process.Start(Path.Combine(TextBox.Text, LeftListView.SelectedItems[0].SubItems[2].Text));
                    return;
                }

                //если выбранный элемент является переходом к родительскому каталогу
                if (LeftListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    try
                    {
                        string parent = Directory.GetParent(LeftViewRootPath).FullName;
                        LeftViewObserver.Path = parent;
                        LeftViewRootPath = parent;
                        TextBox.Text = parent;
                        UpdateListView(LeftListView, LeftViewRootPath);
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("Отказано в доступе!");
                    }
                    return;
                }

                try
                {
                    string child = LeftViewRootPath;
                    if (LeftViewRootPath[LeftViewRootPath.Length - 1] != '\\') child += "\\";
                    child += LeftListView.SelectedItems[0].SubItems[0].Text;
                    LeftViewObserver.Path = child;
                    LeftViewRootPath = child;
                    TextBox.Text = child;
                    UpdateListView(LeftListView, LeftViewRootPath);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Отказано в доступе!");
                }
            }
        }

        private void RightListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
                copy();
            if (e.KeyCode == Keys.X)
                delete();
            if (e.KeyCode == Keys.C)
                create();
            if (e.KeyCode == Keys.Enter)
            {
                //если выбранный элемент не является папкой или переходом к родительскому каталогу, выходим
                if (!LeftListView.SelectedItems[0].SubItems[2].Text.Equals("<папка>") && !LeftListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    if (LeftListView.SelectedItems[0].SubItems[1].Text != "sys")
                    {
                        string p = TextBox.Text + "\\" + LeftListView.SelectedItems[0].SubItems[0].Text + '.' + LeftListView.SelectedItems[0].SubItems[1].Text;
                        TextBox.Text = p;
                        //           string path = "C:\\Games\\BERS.jpg";
                        FileInfo file = new FileInfo(p);
                        Process.Start(p);
                    }
                    //Process.Start(Path.Combine(TextBox.Text, LeftListView.SelectedItems[0].SubItems[2].Text));
                    return;
                }

                //если выбранный элемент является переходом к родительскому каталогу
                if (LeftListView.SelectedItems[0].SubItems[0].Text.Equals("..."))
                {
                    try
                    {
                        string parent = Directory.GetParent(LeftViewRootPath).FullName;
                        LeftViewObserver.Path = parent;
                        LeftViewRootPath = parent;
                        TextBox.Text = parent;
                        UpdateListView(LeftListView, LeftViewRootPath);
                    }
                    catch (FileNotFoundException)
                    {
                        MessageBox.Show("Отказано в доступе!");
                    }
                    return;
                }

                try
                {
                    string child = LeftViewRootPath;
                    if (LeftViewRootPath[LeftViewRootPath.Length - 1] != '\\') child += "\\";
                    child += LeftListView.SelectedItems[0].SubItems[0].Text;
                    LeftViewObserver.Path = child;
                    LeftViewRootPath = child;
                    TextBox.Text = child;
                    UpdateListView(LeftListView, LeftViewRootPath);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("Отказано в доступе!");
                }
            }       
        }
    }
}

