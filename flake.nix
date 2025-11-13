{
  description = ".NET8 tool chain";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/release-24.11";
  };

  outputs =
    { nixpkgs
    , ...
    }@inputs:
    let
      system = "x86_64-linux";
      pkgs = import nixpkgs {
        system = system;
      };

    in

    {
      devShells.${system}.default = pkgs.mkShell {
        packages = with pkgs; [
          dotnet-sdk
        ];

        shellHook = ''
          echo ".NET8 tools loaded"
        '';

        SHELL = "${pkgs.zsh}/bin/zsh";
        DOTNET_ROOT = "${pkgs.dotnet-sdk}/share/dotnet";
      };
    };

}
