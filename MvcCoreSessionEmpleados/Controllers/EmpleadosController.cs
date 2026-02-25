using Microsoft.AspNetCore.Mvc;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SumaSalarial()
        {

            return View();
        }
        public async Task<IActionResult> SessionSalarios(int? salario)
        {
            if (salario != null)
            {
                //QUEREMOS ALMACENAR LA SUMA TOTAL DE SALARIOS
                //QUE TENGAMOS EN SESSION   
                int sumaTotal = 0;
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    //SI YA TENEMOS DATOS ALAMACENADOS LOS RECUPERAMOS

                    sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                //SUMAMOS EL NUEVO SALARIO A LA SUMA TOTAL

                sumaTotal += salario.Value;
                //ALMACENAMOS EL VALOR DENTRO DE SESION

                HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
                ViewData["MENSAJE"] = "SALARIO ALAMACENADO " + salario;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> SessionEmpleados(int? idempleado)
        {
            if (idempleado != null)
            {

                Empleado emple = await this.repo.FindEmpleadoAsync(idempleado.Value);
                //EN SESSION GUARDAREMOS UN COONJUNBTO DE EMPLEADOS
                List<Empleado> empleadosList;
                //DEBEMOS PREGUNTAR SI YA TENEMOS EMPLEADOS EN SESSION

                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    //RECUPERAMOS LA LISTA 

                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    //Creamos una nueva lista para almacenar los empleados

                    empleadosList = new List<Empleado>();

                }
                //AGREGAMOS EL EMPLEADO AL LIST
                empleadosList.Add(emple);
                //ALMACENAMOS LA LISTA EN SESSION

                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = " EMPLEADO: " + emple.Apellido + " alamacenado correctamente";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }
        public async Task<IActionResult> SessionEmpleadosOk(int? idempleado)
        {
            if (idempleado != null)
            {
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    //si no existe se crea la collecion
                    idsEmpleados = new List<int>();
                }
                //almacenamos el id del empleado
                if (!idsEmpleados.Contains(idempleado.Value))
                {
                    idsEmpleados.Add(idempleado.Value);
                    HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    ViewData["MENSAJE"] = " EMPLEADOS ALMACENADOS CORRECTAMENTE " + idsEmpleados.Count;
                }
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosOk()
        {

            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosByIdAsync(idsEmpleados);
                return View(empleados);
            }
        }


        public async Task<IActionResult> SessionEmpleadosV4(int? idempleado)
        {
            List<Empleado> empleados;
            if (idempleado != null)
            {
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                    empleados = await this.repo.GetEmpleadosNoIdSessionAsync(HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS"));
                }
                else
                {
                    //si no existe se crea la collecion
                    idsEmpleados = new List<int>();
                }
                //almacenamos el id del empleado
                if (!idsEmpleados.Contains(idempleado.Value))
                {
                    idsEmpleados.Add(idempleado.Value);
                    HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    ViewData["MENSAJE"] = " EMPLEADOS ALMACENADOS CORRECTAMENTE " + idsEmpleados.Count;
                }
            }

            empleados = await this.repo.GetEmpleadosNoIdSessionAsync(HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS"));
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {

            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosByIdAsync(idsEmpleados);
                return View(empleados);
            }
        }
    }
}
