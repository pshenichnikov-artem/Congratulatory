#!/usr/bin/env bash
set -e

host="$1"
port="$2"
shift 2
cmd="$@"

until nc -z "$host" "$port"; do
  echo "⏳ Waiting for $host:$port to be available..."
  sleep 1
done

echo "✅ $host:$port is available – running command"
exec $cmd
