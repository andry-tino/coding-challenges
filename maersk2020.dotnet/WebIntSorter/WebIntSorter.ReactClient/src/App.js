import React, { useEffect, useState } from 'react';
import { Stack, Text, FontWeights, TextField, ActivityItem, Icon, initializeIcons, DefaultButton, Separator, PrimaryButton, Link, Callout, Spinner, SpinnerSize, mergeStyleSets } from 'office-ui-fabric-react';
import './App.css';
import { array2str, createRandomSequence } from "./utils"

// Do not use http, this will trigger a redirect and CORS does not allow that
const serverAddressHost = `https://localhost`;
const boldStyle = { root: { fontWeight: FontWeights.semibold } };
const normalRandomArraySize = 100;
const longRandomArraySize = 1000;

function App() {
  const [outText, setOutText] = useState("Type your sequence on the left box and then leave...");
  const [srcText, setSrcText] = useState("");
  const [jobId, setJobId] = useState("");
  const [jobState, setJobState] = useState("");
  const [port, setPort] = useState(5001);
  const [calloutPortVisible, setCalloutPortVisible] = useState(false);
  const [calloutClientHostVisible, setCalloutClientHostVisible] = useState(false);
  const [spinnerTopVisible, setSpinnerTopVisible] = useState(false);
  const [spinnerBottomVisible, setSpinnerBottomVisible] = useState(false);
  const [inputTopAreaEnabled, setInputTopAreaEnabled] = useState(true);
  const [inputBottomAreaEnabled, setInputBottomAreaEnabled] = useState(true);
  const [enqueueButtonEnabled, setEnqueueButtonEnabled] = useState(false); // Sync with srcText
  const [infoButtonEnabled, setInfoButtonEnabled] = useState(false); // Sync with jobId

  useEffect(() => {
    updateEnqueueButtonEnabled(srcText);
  }, [srcText]);
  useEffect(() => {
    updateInfoButtonEnabled(jobId);
  }, [jobId]);

  const portLinkCssClassName = "port-link";
  const clientHostLinkCssClassName = "clienthost-link";
  const calloutStyles = mergeStyleSets({
    callout: {
      maxWidth: 300,
      padding: ".5em"
    }
  });
  const textFieldStyles = {
    wrapper: {
      textAlign: "left"
    }
  };
  const activityItemStyles = {
    root: {
      maxWidth: "250px",
      textAlign: "left"
    }
  };

  const clientHost = window.location.host;

  initializeIcons();

  return (
    <Stack
      horizontalAlign="center"
      verticalAlign="center"
      verticalFill
      styles={{
        root: {
          width: "960px",
          margin: "0 auto",
          textAlign: "center",
          color: "#605e5c"
        }
      }}
      gap={15}
    >
      <Text variant="xxLarge" styles={boldStyle}>
        Welcome to WebIntSorter client
      </Text>
    <Text variant="large">Make sure the server is running on port <Link className={portLinkCssClassName} onClick={onPortLinkClick}>{port}</Link> of your localhost. The client is running on <Link className={clientHostLinkCssClassName} onClick={onClientHostLinkClick}>{clientHost}</Link>.</Text>
      <Text variant="large" styles={boldStyle}>
        Enqueue jobs
      </Text>
      <Stack horizontal gap={15} horizontalAlign="center">
        <Stack>
          <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"CheckMark"} />}
            activityDescription={"Make sure the connection settings are ok (page header)."} />
          <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"CheckMark"} />}
            activityDescription={"Type a list manually or use the buttons to generate a random one."} />
          <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"CheckMark"} />}
            activityDescription={"Enqueue a sorting job."} />
          <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"Info"} />}
            activityDescription={"If successful, the job id will be copied in the section below, ready for action."} />
        </Stack>
        <Stack gap={15} verticalAlign="stretch">
          <DefaultButton text="Random sequence" onClick={onButtonRndClick} disabled={!inputTopAreaEnabled}></DefaultButton>
          <DefaultButton text="Long random sequence" onClick={onButtonLongRndClick} disabled={!inputTopAreaEnabled}></DefaultButton>
          <Stack horizontal horizontalAlign="space-between">
            <Stack.Item grow={1}>
              <PrimaryButton text="Enqueue job" disabled={!enqueueButtonEnabled || !inputTopAreaEnabled} onClick={onButtonEnqueueClick}></PrimaryButton>
            </Stack.Item>
            <Stack.Item grow={0}>
              { spinnerTopVisible &&
                <Spinner size={SpinnerSize.medium} />
              }
            </Stack.Item>
          </Stack>
        </Stack>
        <Separator vertical />
        <TextField multiline rows={7} onChange={onSrcTextChange} value={srcText} disabled={!inputTopAreaEnabled}></TextField>
        <TextField multiline readOnly disabled rows={7} value={outText}></TextField>
      </Stack>
      <Separator styles={{root:{width:"100%"}}} />
      <Text variant="large" styles={boldStyle}>
        Inspect jobs
      </Text>
      <Stack horizontal gap={15} horizontalAlign="center">
      <Stack>
        <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"CheckMark"} />}
          activityDescription={"Make sure the connection settings are ok (page header)."} />
        <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"CheckMark"} />}
          activityDescription={"Type a job id in the box."} />
        <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"CheckMark"} />}
          activityDescription={"Request info for the specified job."} />
      </Stack>
        <Stack gap={15}>
          <TextField styles={textFieldStyles} label="Job id" onChange={onJobIdChange} value={jobId} disabled={!inputBottomAreaEnabled}></TextField>
          <Stack horizontal horizontalAlign="space-between">
            <Stack.Item grow={1}>
              <PrimaryButton text="Get job info" disabled={!infoButtonEnabled || !inputBottomAreaEnabled} onClick={onButtonInfoClick}></PrimaryButton>
            </Stack.Item>
            <Stack.Item grow={0}>
              { spinnerBottomVisible &&
                <Spinner size={SpinnerSize.medium} />
              }
            </Stack.Item>
          </Stack>
        </Stack>
        <Separator vertical />
        <TextField multiline readOnly disabled rows={7} value={jobState}></TextField>
      </Stack>
      { calloutPortVisible &&
        <Callout target={`.${portLinkCssClassName}`} onDismiss={onCalloutPortDismissClick} role="alertdialog" gapSpace={0} className={calloutStyles.callout} setInitialFocus>
          <TextField label="Server port" value={port} onChange={onPortTextBoxChange}></TextField>
        </Callout>
      }
      { calloutClientHostVisible &&
        <Callout target={`.${clientHostLinkCssClassName}`} onDismiss={onCalloutClientHostDismissClick} role="alertdialog" gapSpace={0} className={calloutStyles.callout} setInitialFocus>
          <ActivityItem styles={activityItemStyles} activityIcon={<Icon iconName={"AlertSolid"} />}
            activityDescription={"The WebIntSorter server, by default, allows 'localhost:3000' in CORS policy. If this client is running on a different origin, you need to configure the server to allow that."} />
        </Callout>
      }
    </Stack>
  );

  function onPortLinkClick() {
    setCalloutPortVisible(true);
  }

  function onCalloutPortDismissClick() {
    setCalloutPortVisible(false);
  }

  function onClientHostLinkClick() {
    setCalloutClientHostVisible(true);
  }

  function onCalloutClientHostDismissClick() {
    setCalloutClientHostVisible(false);
  }

  function onSrcTextChange(e) {
    const value = e.target.value;
    setSrcText(value);
    updateEnqueueButtonEnabled(value);
  }

  function onJobIdChange(e) {
    const value = e.target.value;
    setJobId(value);
    updateInfoButtonEnabled(value);
  }

  function updateEnqueueButtonEnabled(srcTextValue) {
    setEnqueueButtonEnabled(srcTextValue !== undefined && srcTextValue.length > 0);
  }

  function updateInfoButtonEnabled(jobIdValue) {
    setInfoButtonEnabled(jobIdValue !== undefined && parseInt(jobIdValue) > 0);
  }

  function onPortTextBoxChange(e) {
    const value = e.target.value;
    setPort(value);
  }

  function onButtonEnqueueClick() {
    setOutText("Processing...");
    setInputTopAreaEnabled(false);
    setSpinnerTopVisible(true);

    fetch(`${serverAddressHost}:${port}/api/sorting`, {
      method: "POST",
      mode: "cors",
      headers: {
        "Content-Type": "application/json",
        "Accept": "*/*"
      },
      body: JSON.stringify(
        srcText.split(",").map(x=>+x)
      )
    }).then(res => {
      setOutText("Completed!");
      return res.json();
    }).then(data => {
      const id = data["id"];
      if (id !== undefined) {
        setOutText(`Job id: '${id}'`);
        setJobId(id);
      } else {
        setOutText("Could not retrieve data :(");
      }
    }).catch(err => {
      setOutText(`An error occurred. ${err}`);
    }).finally(() => {
      setInputTopAreaEnabled(true);
      setSpinnerTopVisible(false);
    });
  }

  function onButtonInfoClick() {
    setJobState("Processing...");
    setInputBottomAreaEnabled(false);
    setSpinnerBottomVisible(true);

    fetch(`${serverAddressHost}:${port}/api/sorting/${jobId}`, {
      method: "GET",
      mode: "cors",
      headers: {
        "Content-Type": "application/json",
        "Accept": "*/*"
      }
    }).then(res => {
      setJobState("Completed!");
      return res.json();
    }).then(data => {
      const id = data["id"];
      if (id !== undefined) {
        const msg = `Id: ${id}, Status: ${data["status"]}, Values: '${data["values"]}'`;
        setJobState(msg);
      } else {
        setJobState("Could not retrieve data :(");
      }
    }).catch(err => {
      setJobState(`An error occurred. ${err}`);
    }).finally(() => {
      setInputBottomAreaEnabled(true);
      setSpinnerBottomVisible(false);
    });
  }

  function onButtonRndClick() {
    setSrcText(array2str(createRandomSequence(normalRandomArraySize)));
  }

  function onButtonLongRndClick() {
    setSrcText(array2str(createRandomSequence(longRandomArraySize)));
  }
}

export default App;
