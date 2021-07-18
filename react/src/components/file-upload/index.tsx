import React from "react";
import { Upload } from "tus-js-client";
interface Props {}

const FileUpload = (props: Props) => {
  const onFileChange = (e: any) => {
    console.log(e);
    var file = e.target.files[0];
    const upload = new Upload(file, {
      endpoint: "https://localhost:5001/files",
      retryDelays: [0, 1000, 3000, 5000],
      metadata: {
        filename: file.name,
        filetype: file.type,
      },
      onError: function (error) {
        console.log("Failed because: " + error);
      },
      onProgress: function (bytesUploaded, bytesTotal) {
        var percentage = ((bytesUploaded / bytesTotal) * 100).toFixed(2);
        console.log(bytesUploaded, bytesTotal, percentage + "%");
      },
      onSuccess: function () {
        console.log("Download %s from %s", upload.file.name, upload.url);
      },
    });

    // Check if there are any previous uploads to continue.
    upload.findPreviousUploads().then(function (previousUploads) {
      // Found previous uploads so we select the first one.
      if (previousUploads.length) {
        upload.resumeFromPreviousUpload(previousUploads[0]);
      }

      // Start the upload
      upload.start();
    });
  };
  return (
    <div className="text-center text-2xl p-6">
      <input type="file" name="file" id="" onChange={onFileChange} />
    </div>
  );
};

export default FileUpload;
