# Minimal

**Binacle.Net inside your cluster, reachable only from inside it.** One deployment, a ClusterIP service, your
bin set in a ConfigMap and a persistent volume for the logs. No interactive docs and no web UI - in a cluster
you rarely want either.

What the samples here assume about your cluster is in [the folder above](../README.md).

## 🚀 Run it

Apply them in this order - the deployment mounts both the claim and the ConfigMap:

```bash
kubectl apply -f binacle-pvc.yaml
kubectl apply -f binacle-presets-configmap.yaml
kubectl apply -f binacle-deployment.yaml
kubectl apply -f binacle-net-service.yaml
```

`binacle-pvc.yaml` assumes your cluster provisions volumes dynamically. If it does not, create a matching
PersistentVolume first, or the pod stays pending.

The requests and limits in `binacle-deployment.yaml` are starting values rather than a recommendation. The
file says why there is no CPU limit; measure your own load before trusting either number.

## 🌐 Reaching it

Other pods in the cluster call it by service name:

```text
http://binacle-net-service:8080/
```

From your own machine, forward the port instead of exposing the service:

```bash
kubectl port-forward svc/binacle-net-service 8080:8080
```

## ✏️ Change this first

**The `Presets.json` inside `binacle-presets-configmap.yaml` is your bin set.** The shipped presets are
examples, and until you replace them the answers describe someone else's packaging.

It is mounted with `subPath`, which kubelet never updates in place, so applying the ConfigMap is not enough:

```bash
kubectl apply -f binacle-presets-configmap.yaml
kubectl rollout restart deployment/binacle-net
```

## 💾 It is not stateless

Logs are written to `/app/data`, which is what the claim is for. Delete it and they go with it. Packing
logs are off in this sample - `PackingLogs__Enabled=True` turns them on, and they land in the same place.
