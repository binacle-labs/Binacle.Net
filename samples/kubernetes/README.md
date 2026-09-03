# Kubernetes Samples

Binacle.Net running in the cluster the shops that call it already run in. Copy the folder, edit the manifests,
apply them.

## 📦 Available Samples

- [minimal](minimal) - one deployment, a ClusterIP service, your bin set in a ConfigMap and a volume for the
  logs.

## 📖 What they assume

- You have a cluster, and `kubectl` is pointed at it.
- The callers are in that same cluster - one or more shops, typically behind an Nginx ingress controller.
- Binacle.Net is not exposed outside it. Nothing here creates an ingress or terminates TLS, on purpose: the
  API has no accounts and no authentication unless you turn ServiceModule on, and the
  [docker/service](../docker/service) sample is what that configuration looks like.

Unlike the Docker Compose samples, these are not smoke-tested on release. The image they run is the same one.
