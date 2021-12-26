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
            Workspace workspace = new Workspace("C4 Ing. Software", "sistema de asistencia");
            ViewSet viewSet = workspace.Views;
            Model model = workspace.Model;

            // 1. Diagrama de Contexto
            SoftwareSystem monitoringSystem = model.AddSoftwareSystem("Sistema de registro de asistencias", "Permite el registro de participantes, registro de sus asistencias y la generacion de sus certificados");

            Person ponente = model.AddPerson("Ponente", "Ponente");
            Person organizador = model.AddPerson("Organizador", "encargado");
            Person participante = model.AddPerson("Participante", "Persona");
            
            
            ponente.Uses(monitoringSystem, "Obtiene certificación");
            organizador.Uses(monitoringSystem, "Registra el evento, inscribe perticipantes, gestiona las encuestas y emite certificados.");
            participante.Uses(monitoringSystem, "Registra su participación, obtiene su certificado, rellena encuentas de la ponencia");
            
            SystemContextView contextView = viewSet.CreateSystemContextView(monitoringSystem, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            // Tags
            
            ponente.AddTags("Ponente");
            organizador.AddTags("Organizador");
            participante.AddTags("Participante");
            monitoringSystem.AddTags("SistemaMonitoreo");

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("Participante") { Background = "#6194FA", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Organizador") { Background = "#AF1BFF", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Ponente") { Background = "#FF2641", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("SistemaMonitoreo") { Background = "#79E058", Color = "#ffffff", Shape = Shape.RoundedBox });

            // 2. Diagrama de Contenedores
            Container webApplication = monitoringSystem.AddContainer("Web App", "Permite a los organizadores registrar las asistencias, registrar participantes y generar los certificados.", "Flutter Web");
            Container landingPage = monitoringSystem.AddContainer("Landing Page", "Permite a los usuarios ver las ponencias disponibles", "Flutter Web");
            Container apiRest = monitoringSystem.AddContainer("API Rest", "API Rest", "NodeJS (NestJS) port 8080");
            Container certificateContext = monitoringSystem.AddContainer("Certificate Context", "Bounded Context de certificados", "NodeJS (NestJS)");
            Container participanteContext = monitoringSystem.AddContainer("Participante Context", "Bounded Context de Registro de participantes", "NodeJS (NestJS)");
            Container asistenciaContext = monitoringSystem.AddContainer("Assistance Context", "Bounded Context de registro de asistencias", "NodeJS (NestJS)");
            Container eventContext = monitoringSystem.AddContainer("Event Context", "Bounded Context de registro de eventoos", "NodeJS (NestJS)");
            Container database = monitoringSystem.AddContainer("Database", "", "Oracle");
            
            participante.Uses(landingPage, "Consulta");
            ponente.Uses(landingPage, "Consulta");
            organizador.Uses(webApplication, "Gestiona");

            landingPage.Uses(apiRest, "API Request", "JSON/HTTPS");
            webApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            
            apiRest.Uses(certificateContext, "", "");
            apiRest.Uses(participanteContext, "", "");
            apiRest.Uses(asistenciaContext, "", "");
            apiRest.Uses(eventContext, "", "");
            
            
            certificateContext.Uses(database, "", "JDBC");
            participanteContext.Uses(database, "", "JDBC");
            asistenciaContext.Uses(database, "", "JDBC");
            eventContext.Uses(database, "", "JDBC");

            // Tags
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");
            
            certificateContext.AddTags("CertificateContext");
            participanteContext.AddTags("ParticipanteContext");
            asistenciaContext.AddTags("AsistenciaContext");
            eventContext.AddTags("EventContext");

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

            // 3. Diagrama de Componentes
            Component domainLayer = asistenciaContext.AddComponent("Domain Layer", "", "NodeJS (NestJS)");
            Component registerController = asistenciaContext.AddComponent("Register Controller", "REST API endpoints de registro.", "NodeJS (NestJS) REST Controller");
            Component monitoringApplicationService = asistenciaContext.AddComponent("Assistance Application Service", "Provee métodos para la gestion, pertenece a la capa Application de DDD", "NestJS Component");
            Component asistenciaRepository = asistenciaContext.AddComponent("Assistance Repository", "Información de asistencia", "NestJS Component");
            Component asistentesRepository = asistenciaContext.AddComponent("Asistentes Repository", "Información de los asistentes", "NestJS Component");
            Component certificateRepository = asistenciaContext.AddComponent("Certificados Repository", "generacion del vertificados", "NestJS Component");

            apiRest.Uses(registerController, "", "JSON/HTTPS");
            registerController.Uses(monitoringApplicationService, "Invoca métodos de registro");
            monitoringApplicationService.Uses(domainLayer, "Usa", "");
            monitoringApplicationService.Uses(asistenciaRepository, "", "JDBC");
            monitoringApplicationService.Uses(asistentesRepository, "", "JDBC");
            monitoringApplicationService.Uses(certificateRepository, "", "JDBC");
            asistenciaRepository.Uses(database, "", "JDBC");
            asistentesRepository.Uses(database, "", "JDBC");
            certificateRepository.Uses(database, "", "JDBC");
            
            // Tags
            domainLayer.AddTags("DomainLayer");
            registerController.AddTags("RegisterController");
            monitoringApplicationService.AddTags("MonitoringApplicationService");
            asistenciaRepository.AddTags("AsistenciaRepository");
            asistentesRepository.AddTags("AsistentesRepository");
            certificateRepository.AddTags("CertificateRepository");
            
            styles.Add(new ElementStyle("DomainLayer") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("RegisterController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringApplicationService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringDomainModel") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("FlightStatus") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AsistenciaRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AsistentesRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("CertificateRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentView = viewSet.CreateComponentView(asistenciaContext, "Components", "Component Diagram");
            componentView.PaperSize = PaperSize.A4_Landscape;
            componentView.Add(webApplication);
            componentView.Add(apiRest);
            componentView.Add(database);
            componentView.AddAllComponents();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}