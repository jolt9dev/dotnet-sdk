bash := if os_family() == 'windows' {
    'bash'
} else {
    '/usr/bin/env bash'
}

area cmd name:
    #!{{ bash }}
    echo "Running {{ cmd }} command"
    name="{{ name }}"
    if [ -z "$name" ]; then
        echo "Name is required"
        exit 1
    fi

    case "{{ cmd }}" in
        new|add)
            mkdir -p "./$name"
            dotnet new childbuildprops -o "./$name"
            dotnet new sln -n "$name" -o "./$name"
            ;;
      
        uninstall|remove|r|u)
            dotnet new uninstall ./tpl/bcl
            ;;
        *)
            echo "Invalid command {{ cmd }}"
            ;;
    esac


tpl cmd='add' :
    #!{{ bash }}
    echo "Running {{ cmd }} command"
    case "{{ cmd }}" in
        required|default)
            dotnet new install xunit.v3.templates::0.7.0-pre.15 --force
            dotnet new install Aspire.ProjectTemplates::9.0.0 --force
            ;;

        install|add|i|a|r)
            dotnet new install ./tpl/bcl --force
            dotnet new install ./tpl/child-build-props --force
            ;;
      
        uninstall|remove|r|u)
            dotnet new uninstall ./tpl/bcl
            dotnet new uninstall ./tpl/child-build-props
            ;;
        *)
            echo "Invalid command {{ cmd }}"
            ;;
    esac

bcl cmd name:
    #!{{ bash }}
    echo "Running {{ cmd }} command"
    name="{{ name }}"
    if [ -z "$name" ]; then
        echo "Name is required"
        exit 1
    fi

    case "{{ cmd }}" in


        new|add)
            dotnet new bcl -n "Jolt9.{{ name }}" -o "./bcl/{{ name }}" \
                --no-framework --changelog --use-license-path --use-icon-path  --unsafe --cls
            dotnet sln . add "./bcl/{{ name }}"
            dotnet sln ./bcl/bcl.sln add "./bcl/{{ name }}"
            
            ;;
      
        uninstall|remove|r|u)
            dotnet sln . remove "./bcl/{{ name }}/Jolt9.{{ name }}.csproj"
            dotnet sln ./bcl/bcl.sln remove "./bcl/{{ name }}/Jolt9.{{ name }}.csproj"
            ;;
        *)
            echo "Invalid command {{ cmd }}"
            ;;
    esac