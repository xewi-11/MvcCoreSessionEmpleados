using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;

        public EmpleadosController(RepositoryEmpleados repo,IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
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
            List<int> idsEmpleadosAux2 = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleadosAux2 == null)
            {
                empleados = await this.repo.GetEmpleadosAsync();
                return View(empleados);
            }
            else
            {
                empleados = await this.repo.GetEmpleadosNoIdSessionAsync(HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS"));
                return View(empleados);
            }

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
        
        public async Task<IActionResult> SessionEmpleadosV5(int? idempleado,int? idfavorito)
        {

            if (idfavorito != null)
            {
                //COMO ESTOY ALMACENANDO EN CACHE VAMOS A GUARDAR IRECTAMENTE LOS OBJETOS EN LUGAR DE LOS IDS
                List<Empleado> empleadosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    //No EXISTE NADA EN CACHE
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    //REcuperamos el cache 

                    empleadosFavoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }

                //BUSCAMOS EL EMPLEADO PARA GUARDARLO

                Empleado empleadoFav = await this.repo.FindEmpleadoAsync(idfavorito.Value);
                empleadosFavoritos.Add(empleadoFav);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }



            if (idempleado != null)
            {
                List<int> idsEmpleadosList;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
                {
                    idsEmpleadosList = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    idsEmpleadosList = new List<int>();
                }
                idsEmpleadosList.Add(idempleado.Value);
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleadosList);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleadosList.Count;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        
        public async Task<IActionResult> EmpleadosAlmacenadosV5(int? ideliminar)
        {

            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en session";
                return View();
            }
            else
            {
                if (ideliminar != null)
                {
                    idsEmpleados.Remove(ideliminar.Value);
                    if (idsEmpleados.Count == 0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    }
                   
                }
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosByIdAsync(idsEmpleados);
            return View(empleados);
        }
        public IActionResult EmpleadosFavoritos(int? ideliminar)
        {
            if (ideliminar != null)
            {
                List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                //BUSCAR EMPLEADO PARA ELIMINAR POR SU ID

                Empleado delete = favoritos.Find(z => z.IdEmpleado == ideliminar.Value);
                favoritos.Remove(delete);

                if (favoritos.Count == 0)
                {
                    this.memoryCache.Remove("FAVORITOS");

                }
                else
                {
                    this.memoryCache.Set("FAVORITOS", favoritos);
                }
            }
            return View();
        }
    }
}
