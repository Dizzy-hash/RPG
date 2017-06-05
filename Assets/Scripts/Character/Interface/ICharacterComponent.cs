using UnityEngine;
using System.Collections;

public interface ICharacterComponent
{
    void StartupComponent();
    void ExecuteComponent();
    void ReleaseComponent();
}
