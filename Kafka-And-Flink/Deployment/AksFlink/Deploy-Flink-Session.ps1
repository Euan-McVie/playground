$existingNamespaces = kubectl get namespace
if (-not ($existingNamespaces -match '^flink '))
{
    kubectl create namespace flink
}

$existingServiceAccounts = kubectl get -n flink serviceaccount
if (-not ($existingServiceAccounts -match '^flink '))
{
    kubectl create -n flink serviceaccount flink
    kubectl create clusterrolebinding flink-role-binding-flink --clusterrole=edit --serviceaccount=flink:flink
}

kubectl apply -f $PSScriptRoot\flink-session.yaml

## kubectl port-forward -n flink service/flink-session-rest 6666:8081
