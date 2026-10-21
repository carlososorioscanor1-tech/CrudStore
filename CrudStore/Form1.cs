using CRUDTablas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows.Forms;
//Carlos Osorio 260218, Enzo Argueta 261088, Jose Rivera 260346
namespace CrudStore
{
    public partial class Form1 : Form
    {
        //Carlos Osorio 260218, Enzo Argueta 261088, Jose Rivera 260346
        // Instancia del contexto de la base de datos
        private TiendaContext _context = new TiendaContext();
        private int _productoIdSeleccionado = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Crea de forma nativa el archivo .mdf si se borró
            _context.Database.EnsureCreated();
            cargarCategorias();
            cargarProductos();
        }

        // ==========================================
        // MÉTODOS DE SOPORTE Y LOGICA CRUD
        // ==========================================

        private void cargarCategorias()
        {
            if (!_context.Categorias.Any())
            {
                _context.Categorias.AddRange(
                    new Categoria { Nombre = "Electrónica" },
                    new Categoria { Nombre = "Ropa" }
                );
                _context.SaveChanges();
            }

            cmbCategorias.DataSource = _context.Categorias.ToList();
            cmbCategorias.DisplayMember = "Nombre";
            cmbCategorias.ValueMember = "Id";
        }

        private void cargarProductos()
        {
            var productos = _context.Productos
                .Include(p => p.Categoria)
                .Select(p => new
                {
                    p.Id,
                    Producto = p.Nombre,
                    p.Precio,
                    Categoria = p.Categoria.Nombre,
                    p.CategoriaId
                })
                .ToList();

            dgvProductos.DataSource = productos;

            if (dgvProductos.Columns["CategoriaId"] != null)
            {
                dgvProductos.Columns["CategoriaId"].Visible = false;
            }
        }

        private void limpiarFormulario()
        {
            txtNombre.Clear();
            txtPrecio.Clear();
            _productoIdSeleccionado = 0;
            cmbCategorias.SelectedIndex = 0;
            btnGuardar.Text = "Guardar";
        }

        private void ejecutarGuardado()
        {
            if (string.IsNullOrEmpty(txtNombre.Text) || !decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Por favor, rellene los campos correctamente.");
                return;
            }

            if (_productoIdSeleccionado == 0)
            {
                var nuevoProducto = new Producto
                {
                    Nombre = txtNombre.Text,
                    Precio = precio,
                    CategoriaId = Convert.ToInt32(cmbCategorias.SelectedValue)
                };
                _context.Productos.Add(nuevoProducto);
            }
            else
            {
                var productoExistente = _context.Productos.Find(_productoIdSeleccionado);
                if (productoExistente != null)
                {
                    productoExistente.Nombre = txtNombre.Text;
                    productoExistente.Precio = precio;
                    productoExistente.CategoriaId = Convert.ToInt32(cmbCategorias.SelectedValue);
                }
            }

            _context.SaveChanges();
            cargarProductos();
            limpiarFormulario();
            MessageBox.Show("Operación realizada con éxito.");
        }

        private void ejecutarEliminacion()
        {
            if (_productoIdSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la lista para eliminar.");
                return;
            }

            DialogResult result = MessageBox.Show("¿Está seguro de eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                var producto = _context.Productos.Find(_productoIdSeleccionado);
                if (producto != null)
                {
                    _context.Productos.Remove(producto);
                    _context.SaveChanges();
                    cargarProductos();
                    limpiarFormulario();
                    MessageBox.Show("Producto eliminado con éxito.");
                }
            }
        }

        // ==========================================
        // ENLACES DIRECTOS A LOS BOTONES DEL DISEÑO
        // ==========================================

        // Evento del botón Guardar / Actualizar
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ejecutarGuardado();
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            ejecutarGuardado();
        }

        // Evento del botón Eliminar
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ejecutarEliminacion();
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            ejecutarEliminacion();
        }

        // Evento del botón Limpiar
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiarFormulario();
        }

        private void btnLimpiar_Click_1(object sender, EventArgs e)
        {
            limpiarFormulario();
        }

        // Evento del botón Editar (si aplica limpieza o selección)
        private void btnEditar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Seleccione un elemento de la lista superior para editar sus valores.");
        }

        private void btnEditar_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Seleccione un elemento de la lista superior para editar sus valores.");
        }

        // Evento para cargar datos de la tabla a las cajas de texto al hacer clic
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProductos.Rows[e.RowIndex];
                _productoIdSeleccionado = Convert.ToInt32(row.Cells["Id"].Value);
                txtNombre.Text = row.Cells["Producto"].Value.ToString();
                txtPrecio.Text = row.Cells["Precio"].Value.ToString();
                cmbCategorias.SelectedValue = Convert.ToInt32(row.Cells["CategoriaId"].Value);

                btnGuardar.Text = "Actualizar";
            }
        }
    }
}
