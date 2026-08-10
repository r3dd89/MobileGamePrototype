using UnityEngine;

/*
 * Script Name: MenuScroller
 * Purpose: Slowly scrolls repeating environment sections
 *          on the Main Menu for visual polish.
 */

public class MenuScroller : MonoBehaviour
{
    #region Inspector Settings

    [Header("Environment Sections")]

    [SerializeField] private Transform sectionOne;
    [SerializeField] private Transform sectionTwo;

    [Header("Scrolling Settings")]

    [SerializeField] private float sectionHeight = 10f;

    [SerializeField] private float scrollSpeed = 1.5f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        MoveSection(sectionOne);
        MoveSection(sectionTwo);

        RecycleSection(sectionOne, sectionTwo);
        RecycleSection(sectionTwo, sectionOne);
    }

    #endregion

    #region Scrolling Methods

    private void MoveSection(Transform section)
    {
        if (section == null)
        {
            return;
        }

        section.Translate(
            Vector3.down * scrollSpeed * Time.deltaTime,
            Space.World
        );
    }

    private void RecycleSection(
        Transform section,
        Transform otherSection
    )
    {
        if (section == null || otherSection == null)
        {
            return;
        }

        if (section.position.y <= -sectionHeight)
        {
            Vector3 newPosition = section.position;

            newPosition.y =
                otherSection.position.y + sectionHeight;

            section.position = newPosition;
        }
    }

    #endregion
}