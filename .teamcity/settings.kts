import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.MSBuildStep
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.NUnitStep
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.msBuild
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.nuGetInstaller
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.nunit
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.ScheduleTrigger
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.schedule
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

/*
The settings script is an entry point for defining a TeamCity
project hierarchy. The script should contain a single call to the
project() function with a Project instance or an init function as
an argument.

VcsRoots, BuildTypes, Templates, and subprojects can be
registered inside the project using the vcsRoot(), buildType(),
template(), and subProject() methods respectively.

To debug settings scripts in command-line, run the

    mvnDebug org.jetbrains.teamcity:teamcity-configs-maven-plugin:generate

command and attach your debugger to the port 8000.

To debug in IntelliJ Idea, open the 'Maven Projects' tool window (View
-> Tool Windows -> Maven Projects), find the generate task node
(Plugins -> teamcity-configs -> teamcity-configs:generate), the
'Debug' option is available in the context menu for the task.
*/

version = "2021.1"

project {

    vcsRoot(MappingEDU)

    buildType(MappingEDU02DeployIntegrationSite2)
    buildType(MappingEDUOpenSourceCompileAndTest)

    cleanup {
        baseRule {
            artifacts(builds = 2, artifactPatterns = "+:**/*")
        }
    }
}

object MappingEDU02DeployIntegrationSite2 : BuildType({
    name = "MappingEDU 02 Deploy Integration Site"

    params {
        param("MappingEduPackageName", "MappingEdu.Web.UI")
        param("MappingEduPackageVersion", "${MappingEDUOpenSourceCompileAndTest.depParamRefs["PackageVersion"]}")
    }

    vcs {
        checkoutMode = CheckoutMode.ON_SERVER
    }

    steps {
        step {
            type = "octopus.create.release"
            param("octopus_additionalcommandlinearguments", "--deploymenttimeout=00:10:00 --package=%MappingEduPackageName%:%MappingEduPackageVersion%")
            param("octopus_waitfordeployments", "true")
            param("octopus_version", "2.0+")
            param("octopus_host", "%OctopusServer%")
            param("octopus_project_name", "MappingEDU-OSS")
            param("octopus_deployto", "Integration")
            param("secure:octopus_apikey", "%OctopusAPIKey%")
            param("octopus_releasenumber", "%MappingEduPackageVersion%")
        }
    }

    triggers {
        finishBuildTrigger {
            enabled = false
            buildType = "EdFiBuilds_MappingEDUOpenSource_MappingEDU01CompileTest"
            successfulOnly = true
        }
        schedule {
            schedulingPolicy = weekly {
                dayOfWeek = ScheduleTrigger.DAY.Tuesday
                hour = 15
            }
            branchFilter = ""
            triggerBuild = always()
            param("cronExpression_hour", "15")
            param("cronExpression_dw", "3")
        }
        schedule {
            schedulingPolicy = weekly {
                dayOfWeek = ScheduleTrigger.DAY.Friday
                hour = 15
            }
            branchFilter = ""
            triggerBuild = always()
        }
    }

    dependencies {
        artifacts(MappingEDUOpenSourceCompileAndTest) {
            buildRule = lastSuccessful()
            artifactRules = "+:dist/*.zip!**/*=>dist"
        }
    }
})

object MappingEDUOpenSourceCompileAndTest : BuildType({
    name = "MappingEDU Open Source Compile and Test"
    description = "Compiles and packages the application"

    artifactRules = "build/artifacts/**/*.zip => dist"

    params {
        param("MinorPackageVersion", "1")
        param("testConnectionString", "Server=localhost;Database=MappingEdu_LocalTest;Trusted_Connection=True;MultipleActiveResultSets=true;")
        param("MajorPackageVersion", "1")
        param("PackageVersion", "Populated by build step.")
    }

    vcs {
        root(MappingEDU)

        checkoutMode = CheckoutMode.ON_SERVER
    }

    steps {
        step {
            type = "EdFiBuilds_MappingEDUOpenSource_CalculatePackageVersionDuplicate1"
        }
        nuGetInstaller {
            toolPath = "%teamcity.tool.NuGet.CommandLine.DEFAULT%"
            projects = "src/MappingEdu.sln"
            sources = """
                https://www.nuget.org/api/v2/
                http://www.myget.org/F/d10f0142ad50421a8938b3a9e49977b4/
            """.trimIndent()
            param("nugetCustomPath", "%teamcity.tool.NuGet.CommandLine.DEFAULT%")
            param("nugetPathSelector", "%teamcity.tool.NuGet.CommandLine.DEFAULT%")
        }
        powerShell {
            name = "Build and Package Application"
            workingDir = "src/"
            scriptMode = script {
                content = "msbuild MappingEdu.sln -c release"
            }
        }
        msBuild {
            name = "Create Unit Test Database"
            enabled = false
            path = "build.proj"
            version = MSBuildStep.MSBuildVersion.V12_0
            toolsVersion = MSBuildStep.MSBuildToolsVersion.V12_0
            targets = "MigrateDatabase"
        }
        nunit {
            name = "Unit Tests"
            enabled = false
            nunitPath = "%teamcity.tool.NUnit.Console.DEFAULT%"
            runtimeVersion = NUnitStep.RuntimeVersion.v4_0
            includeTests = """
                src\MappingEdu.Tests.Business\bin\Release\MappingEdu.Tests.Business.dll
                src\MappingEdu.Tests.DataAccess\bin\Release\MappingEdu.Tests.DataAccess.dll
            """.trimIndent()
        }
    }

    triggers {
        vcs {
            enabled = false
        }
    }

    features {
        swabra {
        }
    }
})

object MappingEDU : GitVcsRoot({
    name = "MappingEDU-OSS"
    url = "https://github.com/Ed-Fi-Exchange-OSS/MappingEDU"
    branch = "main"
    branchSpec = """
        +:refs/heads/(*)
        +:refs/(pull/*)/merge
    """.trimIndent()
    authMethod = password {
        userName = "%github.username%"
        password = "%github.accessToken%"
    }
})
