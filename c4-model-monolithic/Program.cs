using Structurizr;
using Structurizr.Api;

namespace c4_model_monolithic
{
    class Program
    {
        static void Main(string[] args)
        {
            Banking();
        }

        static void Banking()
        {
            const long workspaceId = 70829;
            const string apiKey = "9f94426f-5ae7-4048-9008-1baa1d8d6a7a";
            const string apiSecret = "96576e59-fca9-4b3e-89f5-2ee6f23c0cf5";

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);
            Workspace workspace = new Workspace("C4 Ing. Software examen final", "sistema de veterinaria");
            ViewSet viewSet = workspace.Views;
            Model model = workspace.Model;

            // 1. Diagrama de Contexto
            SoftwareSystem monitoringSystem = model.AddSoftwareSystem("Sistema de gestion de citas, ", "Sistema que permite generar citas y registro de los clientes y pacientes(mascotas) ");

            Person cliente = model.AddPerson("Cliente", "Cliente");
            Person macotas = model.AddPerson("Mascotas", "Los pacientes");
            Person usuario = model.AddPerson("Usuario", "Persona que usa el sistema");
            Person doctor = model.AddPerson("Doctor", "Doctor que atienden al paciente");
            
            
            cliente.Uses(monitoringSystem, "LLevan a las macostas a ser atendidas");
            macotas.Uses(monitoringSystem, "Tiene un historial de atenciones");
            usuario.Uses(monitoringSystem, "Registras todas las atenciones y intervenciones del paciente y lo enlaza con la cuenta del cliente");
            doctor.Uses(monitoringSystem, "Atiende a los pacientes y los se lo comunica al usuario para que registro lo atendido");
            
            SystemContextView contextView = viewSet.CreateSystemContextView(monitoringSystem, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            // Tags
            
            cliente.AddTags("Ponente");
            doctor.AddTags("Doctor");
            macotas.AddTags("Organizador");
            usuario.AddTags("Participante");
            monitoringSystem.AddTags("SistemaMonitoreo");

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("Participante") { Background = "#6194FA", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Organizador") { Background = "#AF1BFF", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Ponente") { Background = "#FF2641", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Doctor") { Background = "#008000", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("SistemaMonitoreo") { Background = "#79E058", Color = "#ffffff", Shape = Shape.RoundedBox });

            // 2. Diagrama de Contenedores
            Container webApplication = monitoringSystem.AddContainer("Web App", "Permite a los organizadores registrar las asistencias, registrar participantes y generar los certificados.", "Flutter Web");
            Container MobileApplication = monitoringSystem.AddContainer("Mobile App", "Permite al cliente elegir a su doctor favorito para su mascota", "Flutter Web");
            Container landingPage = monitoringSystem.AddContainer("Landing Page", "Permite a los clientes ver los servicios disponibles", "Flutter Web");
            Container apiRest = monitoringSystem.AddContainer("API Rest", "API Rest", "NodeJS (NestJS) port 8080");
            Container certificateContext = monitoringSystem.AddContainer("Citas Context", "Bounded Context de gestion de citas", "NodeJS (NestJS)");
            Container participanteContext = monitoringSystem.AddContainer("Historial Medico Context", "Bounded Context de Registro de historial medico", "NodeJS (NestJS)");
            Container asistenciaContext = monitoringSystem.AddContainer("Recetas Context", "Bounded Context de registro de recetas", "NodeJS (NestJS)");
            Container eventContext = monitoringSystem.AddContainer("Pacientes Context", "Bounded Context de registro de las mascotas", "NodeJS (NestJS)");
            Container facturacionContext = monitoringSystem.AddContainer("Facturacion Context", "Bounded Context de registro de los comprobantes", "NodeJS (NestJS)");
            Container database = monitoringSystem.AddContainer("Database", "", "Oracle");
            
            usuario.Uses(webApplication, "Gestiona");
            cliente.Uses(landingPage, "Consulta");
            macotas.Uses(landingPage, "consulta");
            cliente.Uses(MobileApplication, "Consulta");
            macotas.Uses(MobileApplication, "consulta");
            doctor.Uses(webApplication, "Gestiona");

            landingPage.Uses(apiRest, "API Request", "JSON/HTTPS");
            MobileApplication.Uses(apiRest, "API Request", "JSON/HTTPS");
            webApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            
            apiRest.Uses(certificateContext, "", "");
            apiRest.Uses(participanteContext, "", "");
            apiRest.Uses(asistenciaContext, "", "");
            apiRest.Uses(eventContext, "", "");
            apiRest.Uses(facturacionContext, "", "");
            
            
            certificateContext.Uses(database, "", "JDBC");
            participanteContext.Uses(database, "", "JDBC");
            asistenciaContext.Uses(database, "", "JDBC");
            eventContext.Uses(database, "", "JDBC");
            facturacionContext.Uses(database, "", "JDBC");

            // Tags
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");
            MobileApplication.AddTags("MobileApp");
            certificateContext.AddTags("CertificateContext");
            participanteContext.AddTags("ParticipanteContext");
            asistenciaContext.AddTags("AsistenciaContext");
            eventContext.AddTags("EventContext");
            facturacionContext.AddTags("EventContext");


            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#929000", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIRest") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("Database") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("AsistenciaContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("ParticipanteContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("CertificateContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("EventContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });

            ContainerView containerView = viewSet.CreateContainerView(monitoringSystem, "Contenedor", "Diagrama de contenedores");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}