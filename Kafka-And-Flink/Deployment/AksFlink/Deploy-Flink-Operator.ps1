# Setup Helm Repo
helm repo add jetstack https://charts.jetstack.io --force-update
helm repo add flink-operator-repo https://downloads.apache.org/flink/flink-kubernetes-operator-1.8.0 --force-update
helm repo update

# Install certificate manager pre-requisite
helm upgrade `
    cert-manager jetstack/cert-manager `
    --namespace cert-manager `
    --create-namespace `
    --install `
    --version v1.14.5 `
    --set installCRDs=true

# Install Flink operator
helm upgrade `
    flink-kubernetes-operator flink-operator-repo/flink-kubernetes-operator `
    --namespace flink-kubernetes-operator `
    --create-namespace `
    --install
