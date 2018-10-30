properties {
    $buildVersionNumber = ""
}

task Publish -depends PublishDocker, HelmInstall

task PublishDocker {
    "Building"
    docker build . -t ncrnolo/travellingdeliveryman:latest

    "Tagging"
    docker tag ncrnolo/travellingdeliveryman:latest ncrnolo/travellingdeliveryman:latest

    "Pushing"
    docker push ncrnolo/travellingdeliveryman:latest
}

task HelmInstall {
    $fullName = docker inspect --format='{{index .RepoDigests 0}}' ncrnolo/travellingdeliveryman:latest
    $tag = $fullName.Split('@')[1]
    "The tag is $tag"
    
    rm *.tgz

    "Packaging"
    helm package Build/Helm/travelling-deliveryman

    "Deleting current release"
    helm delete travelling-prod --purge

    "Publishing new release"
    gci *.tgz | % { "Publishing $_"; helm install --name travelling-prod --set image.tag=$tag $_ }
}