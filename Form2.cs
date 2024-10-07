using Lab04_S7.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Lab04_S7
{

    public partial class Form2 : Form
    {

        DbStudentContent dbStudent = new DbStudentContent();
        private object numTotalProfessor;

        private void FillDataDGV()
        {
            dgvKhoa.Rows.Clear(); // Xóa sạch dữ liệu cũ
            List<Faculty> listFaculty = dbStudent.Faculty.ToList(); // Lấy danh sách khoa từ CSDL
            foreach (var faculty in listFaculty)
            {
                int index = dgvKhoa.Rows.Add();
                dgvKhoa.Rows[index].Cells[0].Value = faculty.FacultyID;  // Mã Khoa
                dgvKhoa.Rows[index].Cells[1].Value = faculty.FacultyName; // Tên Khoa
                dgvKhoa.Rows[index].Cells[2].Value = faculty.TotalProfessor; // Tổng GS
            }
        }


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void dgvKhoa_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckDataInput())
            {
                Faculty newFaculty = new Faculty
                {
                    FacultyName = txtTenKhoa.Text,
                    TotalProfessor = (int?)Convert.ToInt32(txtTongGS.Text) // Cho phép NULL
                };
                dbStudent.Faculty.Add(newFaculty);
                dbStudent.SaveChanges(); // Lưu vào CSDL

                // Hiển thị lại danh sách khoa
                FillDataDGV();
                ClearInputFields();
                MessageBox.Show("Thêm khoa thành công!", "Thông Báo", MessageBoxButtons.OK);
            }
        }

        private bool CheckDataInput()
        {
            if (string.IsNullOrEmpty(txtTenKhoa.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khoa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            int tongGS;
            if (!int.TryParse(txtTongGS.Text, out tongGS) || tongGS < 0)
            {
                MessageBox.Show("Tổng số GS phải là một số nguyên dương!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaKhoa.Text))
            {
                MessageBox.Show("Vui lòng chọn khoa để xóa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int facultyID = Convert.ToInt32(txtMaKhoa.Text);
            Faculty facultyToDelete = dbStudent.Faculty.FirstOrDefault(f => f.FacultyID == facultyID);

            if (facultyToDelete != null)
            {
                dbStudent.Faculty.Remove(facultyToDelete);
                dbStudent.SaveChanges(); // Lưu thay đổi vào CSDL

                // Hiển thị lại danh sách khoa
                FillDataDGV();
                ClearInputFields();
                MessageBox.Show("Xóa khoa thành công!", "Thông Báo", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khoa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputFields()
        {
            txtMaKhoa.Text = "";
            txtTenKhoa.Text = "";
            txtTongGS.Text = "";
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFacultyID.HeaderText))
            {
                MessageBox.Show("Vui lòng chọn khoa để sửa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int facultyID = Convert.ToInt32(txtFacultyID.HeaderText);
            Faculty facultyToUpdate = dbStudent.Faculty.FirstOrDefault(f => f.FacultyID == facultyID);

            if (facultyToUpdate != null)
            {
                facultyToUpdate.FacultyName = txtFacultyName.HeaderText;  // Sửa lại để lấy giá trị Text thay vì HeaderText
                facultyToUpdate.TotalProfessor = numTotalProfessor; // Sử dụng thuộc tính Value của NumericUpDown


                dbStudent.SaveChanges(); // Lưu thay đổi vào CSDL

                // Hiển thị lại danh sách khoa
                FillDataDGV();
                ClearInputFields();
                MessageBox.Show("Sửa khoa thành công!", "Thông Báo", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khoa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
