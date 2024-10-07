using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab04_S7.Model;

namespace Lab04_S7
{
    public partial class Form1 : Form
    {
        DbStudentContent dbStudent = new DbStudentContent();
        private string idNewStudent;

        public Form1()
        {
            InitializeComponent();
            dgvDSSV.CellClick += dgvDSSV_CellClick;
            
        }

        private void dgvDSSV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                DataGridViewRow row = dgvDSSV.Rows[e.RowIndex];

                txtMaSV.Text = row.Cells[0].Value.ToString();  
                txtHoTen.Text = row.Cells[1].Value.ToString(); // Họ tên
                cbbKhoa.Text = row.Cells[2].Value.ToString();  // Khoa
                txtDiemTB.Text = row.Cells[3].Value.ToString(); // Điểm TB
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Student> ListStudent = dbStudent.Student.ToList();
            List<Faculty> ListFaculty = dbStudent.Faculty.ToList();
            FilldataCBB(ListFaculty);
            FilldataDGV(ListStudent);


        }

        private void FilldataDGV(List<Student> listStudent)
        {
            dgvDSSV.Rows.Clear(); 
            foreach (var student in listStudent)
            {
                int RowNew = dgvDSSV.Rows.Add();
                dgvDSSV.Rows[RowNew].Cells[0].Value = student.StudentID;
                dgvDSSV.Rows[RowNew].Cells[1].Value = student.FullName;
                dgvDSSV.Rows[RowNew].Cells[2].Value = student.Faculty.FacultyName;
                dgvDSSV.Rows[RowNew].Cells[3].Value = student.AverageScore;

            }

        }

        private void FilldataCBB(List<Faculty> listFaculty)
        {
            cbbKhoa.DataSource = listFaculty;
            cbbKhoa.DisplayMember = "FacultyName";
            cbbKhoa.ValueMember = "FacultyID";

        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (CheckDataInput())
            {
                if (CheckIdSinhVien(txtMaSV.Text) == -1) 
                {
                    Student newStudent = new Student(); 

                    newStudent.StudentID = txtMaSV.Text;
                    newStudent.FullName = txtHoTen.Text;
                    newStudent.AverageScore = Convert.ToDouble(txtDiemTB.Text);
                    newStudent.FacultyID = Convert.ToInt32(cbbKhoa.SelectedValue.ToString());

                    dbStudent.Student.Add(newStudent);
                    dbStudent.SaveChanges(); 

                    List<Student> ListStudent = dbStudent.Student.ToList();
                    FilldataDGV(ListStudent);

                    
                    MessageBox.Show("Thêm sinh viên thành công!", "Thông Báo", MessageBoxButtons.OK);

                 
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int CheckIdSinhVien(string text)
        {
            int length = dgvDSSV.Rows.Count; 
            for (int i = 0; i < length; i++)
            {
                if (dgvDSSV.Rows[i].Cells[0].Value != null)
                {
                    if (dgvDSSV.Rows[i].Cells[0].Value.ToString() == idNewStudent)
                    {
                        return i; 
                    }
                }
            }
            return -1; 
        }

        private bool CheckDataInput()
        {
            
            if (string.IsNullOrEmpty(txtMaSV.Text) || string.IsNullOrEmpty(txtHoTen.Text) || string.IsNullOrEmpty(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            
            if (txtMaSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 ký tự!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra điểm TB có phải là số hợp lệ hay không
            double diemTB;
            if (!double.TryParse(txtDiemTB.Text, out diemTB) || diemTB < 0 || diemTB > 10)
            {
                MessageBox.Show("Điểm trung bình phải là số từ 0 đến 10!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true; // Dữ liệu hợp lệ
        }

        private void dgvDSSV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu có dòng được chọn
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDSSV.Rows[e.RowIndex]; // Lấy dòng hiện tại

                // Gán các giá trị của dòng được chọn vào các điều khiển trên form
                txtMaSV.Text = row.Cells[0].Value.ToString();  // Mã sinh viên
                txtHoTen.Text = row.Cells[1].Value.ToString(); // Họ tên
                cbbKhoa.Text = row.Cells[2].Value.ToString();  // Khoa (Hiển thị theo tên khoa)
                txtDiemTB.Text = row.Cells[3].Value.ToString(); // Điểm TB
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xóa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị hộp thoại xác nhận xóa
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?", "Xác nhận xóa", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    // Tìm sinh viên trong cơ sở dữ liệu theo Mã số sinh viên
                    string maSV = txtMaSV.Text;
                    Student studentToDelete = dbStudent.Student.FirstOrDefault(s => s.StudentID == maSV);

                    if (studentToDelete != null)
                    {
                        // Xóa sinh viên khỏi cơ sở dữ liệu
                        dbStudent.Student.Remove(studentToDelete);
                        dbStudent.SaveChanges();

                        // Tải lại danh sách sinh viên sau khi xóa
                        List<Student> ListStudent = dbStudent.Student.ToList();
                        FilldataDGV(ListStudent);

                        // Xóa các thông tin hiện tại trong TextBox và ComboBox
                        ClearInputFields();

                        MessageBox.Show("Xóa sinh viên thành công!", "Thông Báo", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên cần xóa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            txtMaSV.Text = "";
            txtHoTen.Text = "";
            cbbKhoa.SelectedValue = dbStudent.Faculty.FirstOrDefault(f => f.FacultyName == "Công Nghệ Thông Tin").FacultyID;
            txtDiemTB.Text = "";
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaSV.Text))
            {
                MessageBox.Show("Vui lòng chọn sinh viên để sửa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hiển thị hộp thoại xác nhận sửa
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn cập nhật thông tin sinh viên này không?", "Xác nhận sửa", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    // Tìm sinh viên trong cơ sở dữ liệu theo Mã số sinh viên
                    string maSV = txtMaSV.Text;
                    Student studentToUpdate = dbStudent.Student.FirstOrDefault(s => s.StudentID == maSV);

                    if (studentToUpdate != null)
                    {
                        // Cập nhật thông tin sinh viên từ các TextBox và ComboBox
                        studentToUpdate.FullName = txtHoTen.Text;
                        studentToUpdate.AverageScore = Convert.ToDouble(txtDiemTB.Text);
                        studentToUpdate.FacultyID = Convert.ToInt32(cbbKhoa.SelectedValue.ToString());

                        // Lưu thay đổi vào cơ sở dữ liệu
                        dbStudent.SaveChanges();

                        // Tải lại danh sách sinh viên sau khi sửa
                        List<Student> ListStudent = dbStudent.Student.ToList();
                        FilldataDGV(ListStudent);

                        // Hiển thị thông báo cập nhật thành công
                        MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông Báo", MessageBoxButtons.OK);

                        // Reset lại các trường nhập liệu sau khi sửa thành công
                        ClearInputFields();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy MSSV cần sửa!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận trước khi thoát
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                // Đóng form và thoát chương trình
                this.Close();
            }
        }

        private void tìmKiếmToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void quảnLýKhoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.ShowDialog();  

        }
    }
}
