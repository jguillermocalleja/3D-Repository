  // World View Projection matrix that will transform the input vertices
  // to screen space.
  float4x4 worldViewProjection : WorldViewProjection;

  // The texture sampler is used to access the texture bitmap
  //in the fragment shader.
  sampler texSampler0;
  uniform float tile;
  // input for our vertex shader
  struct VertexShaderInput {
    float4 position : POSITION;
    // Texture coordinates
  };

  // input for our pixel shader
  struct PixelShaderInput {
    float4 position : POSITION;
    
  };

  /**
   * The vertex shader  transforms input vertices to screen space.
   */
  PixelShaderInput vertexShaderFunction(VertexShaderInput input) {
    PixelShaderInput output;

    // Multiply the vertex positions by the worldViewProjection
    // matrix to transform them to screen space.
    output.position = mul(input.position, worldViewProjection);

    
    return output;
  }

 /**
  * Given the texture coordinates, our pixel shader grabs
  * the corresponding color from the texture.
  */
  float4 pixelShaderFunction(PixelShaderInput input): COLOR {
    
	return float4(0,.5,1,1);
  }

  // Here we tell our effect file *which* functions are
  // our vertex and pixel shaders.

  // #o3d VertexShaderEntryPoint vertexShaderFunction
  // #o3d PixelShaderEntryPoint pixelShaderFunction
  // #o3d MatrixLoadOrder RowMajor