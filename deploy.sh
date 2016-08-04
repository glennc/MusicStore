#!/bin/bash

#this script assumes that the docker client is connected to the cluster (so it can do the deploy), and that the cluster already has the images pulled down.
#In addition to this you need to scale the services on the cluster. So that each service has at least one instance running.

# remove the musicstore stack if it has already been deployed.
docker stack remove musicstore

# Generate a dab file from our compose file
docker-compose bundle -o musicstore.dab

# deploy the stack (this can be done without the stack keyword, but I like the symmetry of remove and add.)
docker stack deploy musicstore

# expose port 80 of the web container so that browsing to the cluster will direct to the containers.
# this maps port 80 on the cluster (from external to the cluster) to port 5000 inside the web container.
docker service update musicstore_web --publish-add 80:5000
