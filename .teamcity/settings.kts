import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.swabra
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.MSBuildStep
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.NUnitStep
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.msBuild
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
    vcsRoot(MappingEDUOss)

    buildType(MappingEDU02DeployIntegrationSite2)
    buildType(MappingEDUOpenSourceCompileAndTest)

    cleanup {
        baseRule {
            artifacts(builds = 2, artifactPatterns = "+:**/*")
        }
    }
}

object MappingEDU02DeployIntegrationSite2 : BuildType({
    name = "MappingEDU OSS Deploy Integration Site"

    params {
        param("MappingEduPackageVersion", "${MappingEDUOpenSourceCompileAndTest.depParamRefs["PackageVersion"]}")
        param("nuget.packages", "dist")
        param("MappingEduPackageName", "MappingEdu.Web.UI")
        param("OctopusProjectName", "MappingEDU-OSS")
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
            param("octopus_project_name", "%OctopusProjectName%")
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
            buildRule = lastSuccessful("+:%teamcity.build.branch%")
            artifactRules = "*.nupkg => %nuget.packages%"
        }
    }
})

object MappingEDUOpenSourceCompileAndTest : BuildType({
    name = "MappingEDU Open Source Compile and Test"
    description = "Compiles and packages the application"

    artifactRules = "*.nupkg"
    publishArtifacts = PublishMode.SUCCESSFUL

    params {
        param("MinorPackageVersion", "2")
        param("testConnectionString", "Server=localhost;Database=MappingEdu_LocalTest;Trusted_Connection=True;MultipleActiveResultSets=true;")
        param("MajorPackageVersion", "1")
        param("msbuildPath", """C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\bin""")
        param("PackageVersion", "Populated by build step.")
    }

    vcs {
        root(MappingEDUOss)

        checkoutMode = CheckoutMode.ON_SERVER
    }

    steps {
        step {
            type = "EdFiBuilds_MappingEDUOpenSource_CalculatePackageVersionDuplicate1"
        }
        powerShell {
            name = "Nuget Restore"
            formatStderrAsError = true
            workingDir = "src/"
            scriptMode = script {
                content = """
                    nuget restore
                    
                    if(${'$'}lastexitcode -ne 0) {
                    	throw "Nuget Restore failed"
                    }
                """.trimIndent()
            }
        }
        powerShell {
            name = "Build  Application"
            formatStderrAsError = true
            workingDir = "src/"
            scriptMode = script {
                content = """
                    ${'$'}env:PATH="${'$'}env:PATH;%msbuildPath%"
                    msbuild -p:Configuration=Release -t:Build
                """.trimIndent()
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
        powerShell {
            name = "Package Application"
            formatStderrAsError = true
            workingDir = "src/MappingEdu.Web.UI"
            scriptMode = script {
                content = """
                    nuget pack MappingEdu.Web.UI.nuspec -OutputDirectory ../../ -Version %PackageVersion% -P Configuration=Release  -Properties NoWarn=NU5100
                    
                    if(${'$'}lastexitcode -ne 0) {
                    	throw "NuGet pack failed"
                    }
                """.trimIndent()
            }
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
    name = "MappingEDU"
    url = "https://github.com/Ed-Fi-Alliance/MappingEDU.git"
    branch = "main"
    branchSpec = """
        refs/heads/(*)
        refs/(pull/*)/merge
    """.trimIndent()
    userNameStyle = GitVcsRoot.UserNameStyle.NAME
    serverSideAutoCRLF = true
    authMethod = password {
        userName = "EdFiBuildAgent"
        password = "zxxd7502a9db2ed52447d5208ad7b0fac68e7ad2247126718b7"
    }
})

object MappingEDUOss : GitVcsRoot({
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
