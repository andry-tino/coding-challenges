import React, { useState } from 'react';
import { Stack, Text, FontWeights, TextField, DefaultButton, Separator, PrimaryButton, Link, Callout, Spinner, SpinnerSize, mergeStyleSets } from 'office-ui-fabric-react';
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
  const [jobId, setOutJobId] = useState("Job id");
  const [jobState, setOutJobState] = useState("");
  const [port, setPort] = useState(5001);
  const [calloutVisible, setCalloutVisible] = useState(false);
  const [spinnerVisible, setSpinnerVisible] = useState(false);
  const [inputEnabled, setInputEnabled] = useState(true);
  const [enqueueButtonEnabled, setEnqueueButtonEnabled] = useState(true);

  const portLinkCssClassName = "port-link";
  const calloutStyles = mergeStyleSets({
    callout: {
      maxWidth: 300,
      padding: ".5em"
    }
  });

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
      <Text variant="large">Make sure the server is running on port <Link className={portLinkCssClassName} onClick={onPortLinkClick}>{port}</Link> your localhost.</Text>
      <Text variant="large" styles={boldStyle}>
        Enqueue jobs
      </Text>
      <Stack horizontal gap={15} horizontalAlign="center">
        <Stack gap={15} verticalAlign="stretch">
          <DefaultButton text="Random sequence" onClick={onButtonRndClick} disabled={!inputEnabled}></DefaultButton>
          <DefaultButton text="Long random sequence" onClick={onButtonLongRndClick} disabled={!inputEnabled}></DefaultButton>
          <Stack horizontal horizontalAlign="space-between">
            <Stack.Item grow={1}>
              <PrimaryButton text="Enqueue job" disabled={!enqueueButtonEnabled || !inputEnabled} onClick={onButtonEnqueueClick}></PrimaryButton>
            </Stack.Item>
            <Stack.Item grow={0}>
              { spinnerVisible &&
                <Spinner size={SpinnerSize.medium} />
              }
            </Stack.Item>
          </Stack>
        </Stack>
        <Separator vertical />
        <TextField multiline rows={7} onChange={updateSrcTextState} value={srcText} disabled={!inputEnabled}></TextField>
        <TextField multiline readOnly disabled rows={7} value={outText}></TextField>
      </Stack>
      <Separator styles={{root:{width:"100%"}}} />
      <Text variant="large" styles={boldStyle}>
        Inspect jobs
      </Text>
      <Stack horizontal gap={15} horizontalAlign="center">
        <TextField value={jobId}></TextField>
        <TextField multiline readOnly disabled rows={7} value={jobState}></TextField>
      </Stack>
      { calloutVisible &&
        <Callout target={`.${portLinkCssClassName}`} onDismiss={onCalloutDismissClick} role="alertdialog" gapSpace={0} className={calloutStyles.callout} setInitialFocus>
          <TextField label="Server port" value={port} onChange={onPortTextBoxChange}></TextField>
        </Callout>
      }
    </Stack>
  );

  function onPortLinkClick() {
    setCalloutVisible(true);
  }

  function onCalloutDismissClick() {
    setCalloutVisible(false);
  }

  function updateSrcTextState(e) {
    setSrcText(e.target.value);
  }

  function onPortTextBoxChange(e) {
    setPort(e.target.value);
  }

  function onButtonEnqueueClick() {
    setOutText("Processing...");
    setInputEnabled(false);
    setSpinnerVisible(true);

    fetch(`${serverAddressHost}:${port}/api/sorting`, {
      method: "POST",
      mode: "cors",
      headers: {
        "Content-Type": "application/json",
        "Accept": "*/*"
      },
      body: JSON.stringify(
        {
          values: srcText.split(",").map(x=>+x)
        }
      )
    }).then(res => {
      setOutText("Hellooooooo");
    }).catch(err => {
      setOutText(`Error: ${err}`);
    }).finally(() => {
      setInputEnabled(true);
      setSpinnerVisible(false);
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
