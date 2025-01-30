# The Community Edition of Neo4j supports running a single database at a time. 
# Additionally, the Community version does not include the capability to assign roles and permissions to users so those steps are not included in this tutorial.
FROM docker.io/library/neo4j:4.4.40-enterprise

USER root

# Create a user and group
RUN addgroup --system -gid 11000 neo4jgroup && \
    adduser --uid 10000 --disabled-password --gecos "" --ingroup neo4jgroup neo4j_account

COPY ./conf/neo4j.conf /var/lib/neo4j/conf/neo4j.conf

# Copy configuration file into the image
COPY ./conf/neo4j.conf /var/lib/neo4j/conf/neo4j.conf

# Update permissions and ownership for configuration file
RUN chown -R 10000:11000 /var/lib/neo4j/conf/neo4j.conf && \
    chmod 640 /var/lib/neo4j/conf/neo4j.conf
    
RUN chown -R 10000:11000 /bin
RUN chown -R 10000:11000 /dev
RUN chown -R 10000:11000 /opt
RUN chown -R 10000:11000 /home
RUN chown -R 10000:11000 /lib
RUN chown -R 10000:11000 /lib64
RUN chown -R 10000:11000 /var
RUN chown -R 10000:11000 /data
RUN chown -R 10000:11000 /logs

USER neo4j_account

CMD ["neo4j"]