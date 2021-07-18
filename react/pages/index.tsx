import React from "react";

import { Container, Header, Main, Footer, Cards } from "@components";
import FileUpload from "@components/file-upload";

const Home: React.FC = () => {
  return (
    <Container>
      <Header />
      <FileUpload></FileUpload>
    </Container>
  );
};

export default Home;
