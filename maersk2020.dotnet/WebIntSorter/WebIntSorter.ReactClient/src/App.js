import React, { useState } from 'react';
import { Stack, Text, FontWeights, TextField, DefaultButton, Separator } from 'office-ui-fabric-react';
import './App.css';
import { array2str, createRandomSequence } from "./utils"

const port = 5001;
const serverAddress = `https://localhost:${port}`;
const boldStyle = { root: { fontWeight: FontWeights.semibold } };
const normalRandomArraySize = 100;
const longRandomArraySize = 1000;

function App() {
  const [outText, setOutText] = useState("Type your sequence on the left box and then leave...");
  const [srcText, setSrcText] = useState("");
  const [jobId, setOutJobId] = useState("Job id");
  const [jobState, setOutJobState] = useState("");

  return (
    <Stack
      horizontalAlign="center"
      verticalAlign="center"
      verticalFill
      styles={{
        root: {
          width: '960px',
          margin: '0 auto',
          textAlign: 'center',
          color: '#605e5c'
        }
      }}
      gap={15}
    >
      <Text variant="xxLarge" styles={boldStyle}>
        Welcome to WebIntSorter client
      </Text>
      <Text variant="large">Make sure the server is running on your localbox on port {port}.</Text>
      <Text variant="large" styles={boldStyle}>
        Enqueue jobs
      </Text>
      <Stack horizontal gap={15} horizontalAlign="center">
        <Stack gap={15} verticalAlign="stretch">
          <DefaultButton text="Random sequence" onClick={onButtonRndClick}></DefaultButton>
          <DefaultButton text="Long random sequence" onClick={onButtonLongRndClick}></DefaultButton>
        </Stack>
        <Separator vertical />
        <TextField multiline rows={7} onChange={updateSrcTextState} onBlur={onSrcBlur} value={srcText}></TextField>
        <TextField multiline readOnly disabled rows={7} value={outText}></TextField>
      </Stack>
      <Separator />
      <Text variant="large" styles={boldStyle}>
        Inspect jobs
      </Text>
      <Stack horizontal gap={15} horizontalAlign="center">
        <TextField value={jobId}></TextField>
        <TextField multiline readOnly disabled rows={7} value={jobState}></TextField>
      </Stack>
    </Stack>
  );

  function updateSrcTextState(e) {
    setSrcText(e.target.value);
  }

  function onSrcBlur() {
    setOutText("Processing...");

    fetch(`${serverAddress}/sorting`, {
      method: "POST",
      body: JSON.stringify(
        {
          values: srcText.split(",").map(x=>+x)
        }
      )
    }).then(res => {
      setOutText("Hellooooooo");
    }).catch(err => {
      setOutText(`Error: ${err}`);
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
