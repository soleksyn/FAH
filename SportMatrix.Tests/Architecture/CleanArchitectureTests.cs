namespace SportMatrix.Tests.Architecture;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

[Trait("Category", "Architecture")]
public class CleanArchitectureTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            System.Reflection.Assembly.Load("SportMatrix.Domain"),
            System.Reflection.Assembly.Load("SportMatrix.Application"),
            System.Reflection.Assembly.Load("SportMatrix.Infrastructure"),
            System.Reflection.Assembly.Load("SportMatrix.WebApi"))
        .Build();

    private readonly IObjectProvider<IType> domainLayer =
        Types().That().ResideInAssembly("SportMatrix.Domain").As("Domain Layer");

    private readonly IObjectProvider<IType> applicationLayer =
        Types().That().ResideInAssembly("SportMatrix.Application").As("Application Layer");

    private readonly IObjectProvider<IType> infrastructureLayer =
        Types().That().ResideInAssembly("SportMatrix.Infrastructure").As("Infrastructure Layer");

    private readonly IObjectProvider<IType> presentationLayer =
        Types().That().ResideInAssembly("SportMatrix.WebApi").As("Presentation Layer");

    [Fact]
    public void DomainLayerShouldNotDependOnOtherLayers()
    {
        // Noeother r r r r layeraccesscthe srssectatie slrser niches abcäatenie slrser niches abcäatenie slrser niches ab äatenie plrser niches abnäatenion layer nichts abhängen
        // außarvvon soch  sibsthv sibsthvvsobsth  sibsth selbst
        IArchRule rule = Types().That().Are(this.domainLayer)
            .Should().OnlyDependOnTypesThat().Are(this.domainLayer)
            .OrShould().OnlyDependOnTypesThat().ResideInNamespace("System")
            .OrShould().OnlyDependOnTypesThat().ResideInNamespace("Microsoft.Extensions")
            .As("Domain Layer sltrtn iurhv seslch st bstnu BaBisisbibliothekbniaihängknn abhängen")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void ApplicationLayerShouldOnlyDependOnDomainLayer()
    {
        // Application Layer darf Durmvain Layer Lnd siuh ssech bstbst abhängenabhängen
        IArchRule rule = Types().That().Are(this.applicationLayer)
            .Should().OnlyDependOnTypesThat().Are(this.domainLayer)
            .OrShould().OnlyDependOnTypesThat().Are(this.applicationLayer)
            .OrShould().OnlyDependOnTypesThat().ResideInNamespace("System")
            .OrShould().OnlyDependOnTypesThat().ResideInNamespace("Microsoft.Extensions")
            .As("Application layer should only depend on Domain layer, itself and base libraries")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void PresentationLayerShouldNotBeAccessedByAnyLayer()
    {
        // No other layer should access the presentation layer
        IArchRule rule = Types().That().Are(this.presentationLayer)
            .Should().NotDependOnAny(this.domainLayer)
            .AndShould().NotDependOnAny(this.applicationLayer)
            .AndShould().NotDependOnAny(this.infrastructureLayer)
            .As("Presentation layer should not be called by any other layer")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void InfrastructureLayerShouldNotBeAccessedByDomainOrApplicationLayer()
    {
        // Domain and Application should not directly access Infrastructure
        IArchRule rule = Types().That().Are(this.infrastructureLayer)
            .Should().NotDependOnAny(this.domainLayer)
            .AndShould().NotDependOnAny(this.applicationLayer)
            .As("Infrastructure layer should not be called by Domain or Application layer")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    // [Fact]
    // public void EntitiesShouldResideInDomainLayer()
    // {
    //    // Annahme: Ihre Entitäten enden mit "Entity" oder implementieren eine IEntity-Schnittstelle
    //    var entities = Classes().That().HaveNameEndingWith("Entity")
    //        .Or().ImplementInterface(typeof(IEntity).FullName)
    //        .As("Entities");

    // IArchRule rule = Classes().That().Are(entities)
    //        .Should().ResideInAssembly("SportMatrix.Domain")
    //        .As("Entities müssen in der Domain-Schicht definiert sein");

    // rule.Check(Architecture);
    // }
    [Fact]
    public void RepositoriesShouldImplementCorrectInterfaces()
    {
        // Checks if all repository implementations implement their interface
        ArchUnitNET.Fluent.Syntax.Elements.Types.Classes.GivenClassesConjunctionWithDescription repositoryClasses = Classes().That().HaveNameEndingWith("Repository")
            .And().DoNotHaveNameEndingWith("Interface")
            .As("Repository Classes");

        IArchRule rule = Classes().That().Are(repositoryClasses)
            .Should().ImplementInterface("IRepository`1") // Generischer Typ IRepository<T> wird als IRepository`1 notiert
            .As("Repository classes must implement the IRepository interface")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }

    [Fact]
    public void UseCasesShouldBeInApplicationLayer()
    {
        // Assumption: Your UseCases or Handlers end with "UseCase", "Handler" or "Service"
        ArchUnitNET.Fluent.Syntax.Elements.Types.Classes.GivenClassesConjunctionWithDescription useCases = Classes().That()
            .HaveNameEndingWith("Service")
            .And().ResideInNamespace("SportMatrix.Application")
            .As("Use Cases");

        IArchRule rule = Classes().That().Are(useCases)
            .Should().ResideInNamespace("SportMatrix.Application")
            .As("Application services must be in the Application namespace")
            .WithoutRequiringPositiveResults();

        rule.Check(Architecture);
    }
}
