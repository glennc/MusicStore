# Music Store

The intent of this fork is to break up Music Store into smaller logical pieces and deploy allow it to be deployed to a Docker Swarm Mode cluster, being able to take advantage of scalability and other features of containerisation.

You can see the journey via the tags in the repo named the same as each of the following headings:

### Stage1 (proof of concept):

This is the "proof that we can get something working" stage.

At this point in the process we have succesfully factored out a few of the queries that Music Store is making into a Web API service. However, we are just using a SQLite database that we seed on app start.

Gains:
	- Proved we can get something workin on a swarm mode cluster, which means the two services need to be able to talk to each other and we need to be able to hit the web front end from outside the cluster. There are some configuration steps on the cluster to make this work, but it is possible with the code at this label.
	- The deploy.sh script shows the commands that can deploy the app. It assumes a cluster is already configured.

Obvious Problems in Stage1:
	- Logging is currently really bad. It is possible to see the console logs of a running container, but when running on a cluster it is difficult to do that (I ended up using SSH to access each node so that I could do `docker logs <id>` on whatever node the container ended up on.)
	- Data acess is obvious a problem, we will need to decide how we want to handle that.

### Stage2 (Fix Logging):

### Stage3 (Fix data access):

### Stage 4 (Profit?):