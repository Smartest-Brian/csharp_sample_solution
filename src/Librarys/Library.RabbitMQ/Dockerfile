FROM rabbitmq:3-management
COPY definitions.json /etc/rabbitmq/definitions.json
ENV RABBITMQ_LOAD_DEFINITIONS=true \
    RABBITMQ_DEFINITIONS_FILE=/etc/rabbitmq/definitions.json
