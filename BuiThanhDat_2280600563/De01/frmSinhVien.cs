using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using De01.model;

namespace De01
{
    public partial class frmSinhVien : Form
    {
        private StudentContextDB context;

        public frmSinhVien()
        {
            InitializeComponent();
            context = new StudentContextDB();
            LoadClass();
            LoadData();
        }

        private void LoadClass()
        {
            var classList = context.Lops.ToList();
            cmbClass.DataSource = classList;
            cmbClass.DisplayMember = "TenLop";
            cmbClass.ValueMember = "MaLop";
        }

        private void LoadData()
        {
            var studentList = context.Sinhviens.Include(s => s.Lop).ToList();
            dgvStudentList.AutoGenerateColumns = false;
            dgvStudentList.Columns.Clear();

            // Định nghĩa các cột cho DataGridView
            dgvStudentList.Columns.Add("MaSV", "Mã SV");
            dgvStudentList.Columns.Add("HotenSV", "Họ và Tên");
            dgvStudentList.Columns.Add("NgaySinh", "Ngày Sinh");
            dgvStudentList.Columns.Add("TenLopHoc", "Lớp");

            // Gán DataPropertyName cho các cột
            dgvStudentList.Columns["MaSV"].DataPropertyName = "MaSV";
            dgvStudentList.Columns["HotenSV"].DataPropertyName = "HotenSV";
            dgvStudentList.Columns["NgaySinh"].DataPropertyName = "NgaySinh";
            dgvStudentList.Columns["TenLopHoc"].DataPropertyName = "TenLopHoc"; 

            // Chiều rộng các cột
            dgvStudentList.Columns["HotenSV"].Width = 150;
            dgvStudentList.Columns["TenLopHoc"].Width = 150;
            dgvStudentList.Columns["NgaySinh"].Width = 150;

            dgvStudentList.DataSource = studentList;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var newStudent = new Sinhvien
                {
                    MaSV = txtStudentID.Text,
                    HotenSV = txtName.Text,
                    NgaySinh = dtpkDateofBirth.Value,
                    MaLop = cmbClass.SelectedValue.ToString(),
                };

                context.Sinhviens.Add(newStudent);
                context.SaveChanges();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi trong quá trình thêm sinh viên: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không ?? ", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    if (dgvStudentList.SelectedRows.Count > 0)
                    {
                        var studentToDelete = (Sinhvien)dgvStudentList.SelectedRows[0].DataBoundItem;
                        context.Sinhviens.Remove(studentToDelete);
                        context.SaveChanges();
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi trong quá trình xóa sinh viên: " + ex.Message);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvStudentList.SelectedRows.Count > 0)
                {
                    var studentToUpdate = (Sinhvien)dgvStudentList.SelectedRows[0].DataBoundItem;

                    studentToUpdate.HotenSV = txtName.Text;
                    studentToUpdate.NgaySinh = dtpkDateofBirth.Value;
                    studentToUpdate.MaLop = cmbClass.SelectedValue.ToString();

                    context.SaveChanges();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi trong quá trình cập nhật sinh viên: " + ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void dgvStudentList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStudentList.SelectedRows.Count > 0)
            {
                var selectedStudent = (Sinhvien)dgvStudentList.SelectedRows[0].DataBoundItem;
                txtStudentID.Text = selectedStudent.MaSV;
                txtName.Text = selectedStudent.HotenSV;
                dtpkDateofBirth.Value = selectedStudent.NgaySinh ?? DateTime.Now;
                cmbClass.SelectedValue = selectedStudent.MaLop;
            }
        }

        private void btnFindByName_Click(object sender, EventArgs e)
        {
            string searchTerm = txtFindByName.Text.Trim();

            var filteredStudents = context.Sinhviens
                                          .Include(s => s.Lop)
                                          .Where(s => s.HotenSV.Contains(searchTerm))
                                          .ToList();

            dgvStudentList.DataSource = filteredStudents;
        }
    }
}