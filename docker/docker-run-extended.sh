#!/bin/bash
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.



# This is the entry point for the docker images.
# This file is executed when docker run is called.


set -e

BROKER_HOME=/var/lib/
CONFIG_PATH=$BROKER_HOME/etc
export BROKER_HOME OVERRIDE_PATH CONFIG_PATH

if [[ ${ANONYMOUS_LOGIN,,} == "true" ]]; then
  LOGIN_OPTION="--allow-anonymous"
else
  LOGIN_OPTION="--require-login"
fi

CREATE_ARGUMENTS="--user ${ARTEMIS_USER} --password ${ARTEMIS_PASSWORD} --silent ${LOGIN_OPTION} ${EXTRA_ARGS}"

echo CREATE_ARGUMENTS=${CREATE_ARGUMENTS}

if ! [ -f ./etc/broker.xml ]; then
    /opt/activemq-artemis/bin/artemis create ${CREATE_ARGUMENTS} .

    if [ "$REDISTRIBUTIONDELAY" ]; then
        echo Enabling message redistribution with delay of $REDISTRIBUTIONDELAY

        xmlstarlet ed -L \
            -N activemq="urn:activemq" \
            -N core="urn:activemq:core" \
            -s "/activemq:configuration/core:core/core:address-settings/core:address-setting[@match='#']"  \
            -t elem \
            -n "redistribution-delay" \
            -v "$REDISTRIBUTIONDELAY" etc/broker.xml
    fi

    if [ "$SCALEDOWN" = "true" ]; then
        echo Enabling scale down with discovery group 'dg-group1'

        xmlstarlet ed -L \
            -N activemq="urn:activemq" \
            -N core="urn:activemq:core" \
            -s "/activemq:configuration/core:core" -t elem -n "ha-policy" \
            -s "/activemq:configuration/core:core/ha-policy" -t elem -n "live-only" \
            -s "/activemq:configuration/core:core/ha-policy/live-only" -t elem -n "scale-down" \
            -s "/activemq:configuration/core:core/ha-policy/live-only/scale-down" -t elem -n "enabled" -v "true" \
            -s "/activemq:configuration/core:core/ha-policy/live-only/scale-down" -t elem -n "discovery-group-ref" \
            -s "/activemq:configuration/core:core/ha-policy/live-only/scale-down/discovery-group-ref" -t attr -n "discovery-group-name" -v "dg-group1" \
            etc/broker.xml
    fi
else
    echo "broker already created, ignoring creation"
fi

exec ./bin/artemis "$@"

