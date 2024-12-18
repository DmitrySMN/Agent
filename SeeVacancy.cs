﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace Agent
{
    public partial class SeeVacancy : Form
    {
        
        int currentRowIndex;
        int currentColumnIndex;

        string roleEmp;
        string searchIn;
        int resume;
        string profession;
        int applicantId;
        //Добавить возможность у администратора редактировать и удалять вакансии при нажатии на правую кнопку мыши
        public SeeVacancy(int idResume=0, string profession = "")
        {
            InitializeComponent();

            resume = idResume;
            roleEmp = func.search($"SELECT employe_post FROM employe WHERE id = {port.empIds}");

            searchIn = $@"SELECT vacancy.id, company.company_name as 'Комапния', profession.name as 'Профессия', vacancy.vacancy_responsibilities as 'Обязанности', vacancy.vacancy_requirements as 'Требования', vacancy.vacancy_conditions as 'Условия',   CONCAT( vacancy.vacancy_salary_by, ' - ', vacancy.vacancy_salary_before) as 'Размер зарплаты', vacancy.vacancy_address as 'Адресс работы', vacancy.vacancy_delete_status as 'Status', companyc_linq as 'Cсылка'   
                        FROM vacancy 
                        INNER JOIN company ON vacancy.vacancy_company = company.id 
                        INNER JOIN profession ON vacancy.vacancy_profession = profession.id";
            if (roleEmp == "2" ) 
            {
                if (resume == 0)
                {
                    textBoxSearch.Visible = true;
                    comboBox1.Visible = true;
                    comboBox2.Visible = true;
                }
                else
                {
                    applicantId = Convert.ToInt32(func.search($"SELECT resume_applicant FROM resume WHERE id = {idResume}"));
                    searchIn += $" WHERE profession.name = '{profession}' ";
                }
                
            }
            
        }
        string Search()
        {
            string basis = "SELECT vacancy.id, company.company_name as 'Комапния', profession.name as 'Профессия', vacancy.vacancy_responsibilities as 'Обязанности', vacancy.vacancy_requirements as 'Требования', vacancy.vacancy_conditions as 'Условия', CONCAT( vacancy.vacancy_salary_by, ' - ', vacancy.vacancy_salary_before) as 'Размер зарплаты',  vacancy.vacancy_address as 'Адресс работы',  vacancy.vacancy_delete_status as 'Status',  companyc_linq as 'Cсылка' " +
                            "FROM vacancy " +
                            "INNER JOIN company ON vacancy.vacancy_company = company.id " +
                            "INNER JOIN profession ON vacancy.vacancy_profession = profession.id ";
            if ((comboBox2.SelectedIndex != -1 && comboBox2.SelectedIndex != 0 || textBoxSearch.Text.Length > 0)&& resume==0)
            {
                basis += "WHERE ";
            }
            if (comboBox2.SelectedIndex != -1 && comboBox2.SelectedIndex != 0)
            {
                basis += $"(profession.name = '{comboBox2.SelectedItem}')";
            }
            if (textBoxSearch.Text.Length > 0)
            {
                if (comboBox2.SelectedIndex != -1 && comboBox2.SelectedIndex != 0)
                    basis += " AND ";
                basis += $"(company.company_name LIKE '%{textBoxSearch.Text}%' OR vacancy.vacancy_requirements LIKE '%{textBoxSearch.Text}%' OR vacancy.vacancy_conditions LIKE '%{textBoxSearch.Text}%' OR  vacancy.vacancy_responsibilities LIKE '%{textBoxSearch.Text}%')";
            }
            if (comboBox1.SelectedIndex == 0)
            {
                basis += $"ORDER BY vacancy.vacancy_salary_by";
            } else if (comboBox1.SelectedIndex == 1)
            {
                basis += $"ORDER BY vacancy.vacancy_salary_by DESC";
            }
            return basis;
        }
        void load_load()
        {
            func.load(dataGridView1, searchIn);
            dataGridView1.Columns["Комапния"].Width = 160;
            dataGridView1.Columns["Профессия"].Width = 200;
            dataGridView1.Columns["Обязанности"].Width = 340;
            dataGridView1.Columns["Требования"].Width = 270;
            dataGridView1.Columns["Условия"].Width = 270;
            dataGridView1.Columns["Размер зарплаты"].Width = 150;
            dataGridView1.Columns["Адресс работы"].Width = 310;
            dataGridView1.Columns["id"].Visible = false;

            dataGridView1.Columns["Status"].Visible = false;
            dataGridView1.Columns["Cсылка"].Visible = false;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            CurrencyManager manager = (CurrencyManager)BindingContext[dataGridView1.DataSource];
            manager.SuspendBinding();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string status = row.Cells["Status"].Value.ToString();
                if (status == "1" || status == "2")
                    row.Visible = false;

            }
            manager.ResumeBinding();
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (System.Uri.IsWellFormedUriString(r.Cells["Cсылка"].Value.ToString(), UriKind.Absolute))
                {
                    r.Cells["Комапния"] = new DataGridViewLinkCell();
                    DataGridViewLinkCell c = r.Cells["Комапния"] as DataGridViewLinkCell;
                    c.LinkColor = Color.Green;

                }
            }
        }
        private void SeeVacancy_Load(object sender, EventArgs e)
        {
            if (resume != 0)
            {
                string searcNow = $@"SELECT resume.id, CONCAT(applicant.applicant_surname, ' ',applicant.applicant_name, ' ', applicant.applicant_patronymic) as 'Соискатель', profession.name as 'Профессия', resume.resume_knowledge_of_languages as 'Знание языков', resume.resume_personal_qualities as 'Личностные качества', resume.salary as 'Зарплата', applicant.applicant_delete_status as 'Status'
                        FROM resume  
                        INNER JOIN applicant ON resume.resume_applicant = applicant.applicant_id 
                        INNER JOIN profession ON resume.resume_profession = profession.id 
                        WHERE resume.id = '{resume}'";
                func.load(dataGridView2, searcNow);
                dataGridView2.Columns["id"].Visible = false;
                dataGridView2.Columns["Status"].Visible = false;
                dataGridView2.Columns["Соискатель"].Width = 270;
                dataGridView2.Columns["Профессия"].Width = 250;
                dataGridView2.Columns["Знание языков"].Width = 208;
                dataGridView2.Columns["Личностные качества"].Width = 300;
                dataGridView2.Columns["Зарплата"].Width = 140;




                dataGridView2.Visible = true;
            }

            

            labelFIO.Text = func.search($"SELECT CONCAT(employe_surname, ' ', employe_name, ' ', employe_partronymic) FROM employe WHERE id = '{port.empIds}'");
            comboBox2.Items.Add("Без фильтра");
            List<string> list = new List<string>();
            MySqlConnection connection = new MySqlConnection(Connection.connect());
            connection.Open();
            string find = $"SELECT name FROM profession;";
            MySqlCommand com = new MySqlCommand(find, connection);
            var pwd = "";
            MySqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {

                if (!comboBox2.Items.Contains(reader[0].ToString()))
                    comboBox2.Items.Add(reader[0].ToString());
            }
            connection.Close();

            comboBox1.Items.Add("По возрастанию зарплаты");
            comboBox1.Items.Add("По убыванию зарплаты");
            load_load();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            if (roleEmp == "2")
            {
                if (resume == 0)
                {
                    MenuManager menuManager = new MenuManager();
                    menuManager.Show();
                    this.Close();
                }
                else
                {
                    res res = new res(0, resume);
                    res.Show();
                    this.Close();
                }
            }
            else
            {
                MenuAdmin menuA = new MenuAdmin();
                menuA.Show();
                this.Close();
            }
            
            
        }
        void change()
        {
            CurrencyManager manager = (CurrencyManager)BindingContext[dataGridView1.DataSource];
            manager.SuspendBinding();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string status = row.Cells["Status"].Value.ToString();
                if (status == "1" || status == "2")
                    row.Visible = false;
            }
            manager.ResumeBinding();
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (System.Uri.IsWellFormedUriString(r.Cells["Cсылка"].Value.ToString(), UriKind.Absolute))
                {
                    r.Cells["Комапния"] = new DataGridViewLinkCell();
                    DataGridViewLinkCell c = r.Cells["Комапния"] as DataGridViewLinkCell;
                    c.LinkColor = Color.Green;

                }
            }
            dataGridView1.Columns["Комапния"].Width = 160;
            dataGridView1.Columns["Профессия"].Width = 200;
            dataGridView1.Columns["Обязанности"].Width = 340;
            dataGridView1.Columns["Требования"].Width = 270;
            dataGridView1.Columns["Условия"].Width = 270;
            dataGridView1.Columns["Размер зарплаты"].Width = 150;
            dataGridView1.Columns["Адресс работы"].Width = 310;
            dataGridView1.Columns["id"].Visible = false;

            dataGridView1.Columns["Status"].Visible = false;
            dataGridView1.Columns["Cсылка"].Visible = false;
        }
        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            func.load(dataGridView1, Search());
            dataGridView1.ClearSelection();


            change();
            

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            func.load(dataGridView1, Search());
            change();
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            func.load(dataGridView1, Search());
            change();
            
        }
        void menu(object sender, MouseEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            this.currentColumnIndex = dataGridView1.HitTest(e.X, e.Y).ColumnIndex;
            this.currentRowIndex = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            if(e.Button == MouseButtons.Right)
            {
                if (roleEmp == "2")
                {
                    if (resume == 0)
                        contextMenu.MenuItems.Add(new MenuItem("Выбрать соискателя", click_to));
                    else
                        contextMenu.MenuItems.Add(new MenuItem("Создать направление", dir));
                    
                }
                else
                {
                    contextMenu.MenuItems.Add(new MenuItem("Редактировать вакансию", update));
                    contextMenu.MenuItems.Add(new MenuItem("Удалить вакансию", delete));
                }
                if (currentRowIndex >= 0)
                {
                    dataGridView1.Rows[currentRowIndex].Selected = true;
                    contextMenu.Show(dataGridView1, new Point(e.X, e.Y));
                }
                else
                {
                    currentRowIndex = 0;


                }

            }
            
        }
        void dir(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Создать направление с этим резюме?",
               "Подтверждение",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Information
               );
            if (result == DialogResult.Yes)
            {
                string vacancyProfession = dataGridView1.Rows[currentRowIndex].Cells["Профессия"].Value.ToString();
                int vacancyID = Convert.ToInt32(dataGridView1.Rows[currentRowIndex].Cells["id"].Value);
                DateTime now = DateTime.Now;
                func.direction($"INSERT INTO direction(direction_aplicant,direction_vacancy,direction_employee,direction_date,direction_status) SELECT'{applicantId}','{vacancyID}','{port.empIds}','{now.ToString("yyyy-MM-dd")}','Ожидание' WHERE NOT EXISTS ( SELECT 1 FROM direction WHERE direction_aplicant = '{applicantId}' AND direction_vacancy = '{vacancyID}' AND  direction_status = 'Ожидание');");
                MessageBox.Show(
              "Направление успешно создано",
              "Уведомление"
              );
                SeeResume resume = new SeeResume();
                resume.Show();
                this.Close();
            }
        }

        void click_to(object sender, EventArgs e)
        {
            string vacancyProfession = dataGridView1.Rows[currentRowIndex].Cells["Профессия"].Value.ToString();
            int vacancyID = Convert.ToInt32(dataGridView1.Rows[currentRowIndex].Cells["id"].Value);
            SeeResume seeResume = new SeeResume( vacancyProfession,vacancyID);
            seeResume.Show();
            this.Hide();
            dataGridView1.Rows[currentRowIndex].Selected = false;
        }
        void update(object sender, EventArgs e)
        {
            int vacancyID = Convert.ToInt32(dataGridView1.Rows[currentRowIndex].Cells["id"].Value);
            AddV addV = new AddV(0,vacancyID);
            addV.Show();
            this.Close();
            
        }
        void delete(object sender, EventArgs e)
        {
            int vacancyID = Convert.ToInt32(dataGridView1.Rows[currentRowIndex].Cells["id"].Value);
            DialogResult results = MessageBox.Show(
               "Вы действительно хотите удалить вакансию?",
               "Подтверждение",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Information
               );
            if (results == DialogResult.Yes)
            {
                func.direction($@"UPDATE vacancy
                             SET vacancy_delete_status = 1
                             WHERE id = {vacancyID}");
                MessageBox.Show("Ваканисия успешно удалена", "Уведомление");
                load_load();
            }
            
        }
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            menu(sender, e);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                string link = dataGridView1.Rows[e.RowIndex].Cells["Cсылка"].Value.ToString();
                if (link != "")
                {
                    try
                    {
                        System.Diagnostics.Process.Start(link);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ссылка указана не верно. Удалите или измените ссылку", "Ошибка");
                    }

                }

            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SeeVacancy_Paint(object sender, PaintEventArgs e)
        {
            func.FormPaint(this);
        }
    }
}
