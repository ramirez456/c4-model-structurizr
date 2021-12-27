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

            // 3. Diagrama de Componentes asistencias
            Component domainLayer = asistenciaContext.AddComponent("Domain Layer", "", "NodeJS (NestJS)");
            Component registerController = asistenciaContext.AddComponent("Register Controller", "REST API endpoints de registro.", "NodeJS (NestJS) REST Controller");
            Component assistanceApplicationService = asistenciaContext.AddComponent("Assistance Application Service", "Provee métodos para la gestion, pertenece a la capa Application de DDD", "NestJS Component");
            Component asistenciaEntity = asistenciaContext.AddComponent("Assistance Entity", "model para  Asistencias", "NestJS Component");
            Component asistenciaMaper = asistenciaContext.AddComponent("Assistance Maper", "Tipado de las request", "NestJS Component");

            apiRest.Uses(registerController, "", "JSON/HTTPS");
            registerController.Uses(assistanceApplicationService, "Invoca métodos de registro");
            assistanceApplicationService.Uses(domainLayer, "Usa", "");
            assistanceApplicationService.Uses(asistenciaEntity, "", "JDBC");
            assistanceApplicationService.Uses(asistenciaMaper, "", "JDBC");
            asistenciaEntity.Uses(database, "", "JDBC");
            asistenciaMaper.Uses(database, "", "JDBC");
            
            // Tags
            domainLayer.AddTags("DomainLayer");
            registerController.AddTags("RegisterController");
            assistanceApplicationService.AddTags("assistanceApplicationService");
            asistenciaEntity.AddTags("AsistenciaEntity");
            asistenciaMaper.AddTags("AsistenciaMaper");
            
            styles.Add(new ElementStyle("DomainLayer") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("RegisterController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("assistanceApplicationService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringDomainModel") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AsistenciaEntity") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AsistenciaMaper") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentView = viewSet.CreateComponentView(asistenciaContext, "Component", "Component Diagram");
            componentView.PaperSize = PaperSize.A4_Landscape;
            componentView.Add(webApplication);
            componentView.Add(apiRest);
            componentView.Add(database);
            componentView.AddAllComponents();

            //4.- componentes eventos
            Component domainEventLayer = eventContext.AddComponent("Domain Layer", "", "NodeJS (NestJS)");
            Component eventController = eventContext.AddComponent("Register Controller", "REST API endpoints de registro.", "NodeJS (NestJS) REST Controller");
            Component eventApplicationService = eventContext.AddComponent("Event Application Service", "Provee métodos para la gestion, pertenece a la capa Application de DDD", "NestJS Component");
            Component eventEntity = eventContext.AddComponent("Event Entity", "model para  event", "NestJS Component");
            Component eventMaper = eventContext.AddComponent("Event Maper", "Tipado de las request", "NestJS Component");

            apiRest.Uses(eventController, "", "JSON/HTTPS");
            eventController.Uses(eventApplicationService, "Invoca métodos de registro");
            eventApplicationService.Uses(domainEventLayer, "Usa", "");
            eventApplicationService.Uses(eventEntity, "", "JDBC");
            eventApplicationService.Uses(eventMaper, "", "JDBC");
            eventEntity.Uses(database, "", "JDBC");
            eventMaper.Uses(database, "", "JDBC");
            
            // Tags
            domainEventLayer.AddTags("domainEventLayer");
            eventController.AddTags("eventController");
            eventApplicationService.AddTags("eventApplicationService");
            eventEntity.AddTags("eventEntity");
            eventMaper.AddTags("eventMaper");
            
            styles.Add(new ElementStyle("domainEventLayer") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("eventController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("eventApplicationService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("eventEntity") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("eventMaper") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentEventView = viewSet.CreateComponentView(eventContext, "Components", "Component Diagram");
            componentEventView.PaperSize = PaperSize.A4_Landscape;
            componentEventView.Add(webApplication);
            componentEventView.Add(apiRest);
            componentEventView.Add(database);
            componentEventView.AddAllComponents();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}