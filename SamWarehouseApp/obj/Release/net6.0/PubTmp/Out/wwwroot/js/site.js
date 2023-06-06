// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var html = document.querySelector("html")
var body = document.querySelector('body')
var theme = localStorage.getItem("theme")

validateTheme(theme)

function storeSuccess() {
    localStorage.setItem('result', 'success')
}

function storeFaiure() {
    localStorage.setItem('result', 'fail');
}


//switches the theme and applyes styles to the page that match the teme effect. 
function switchTheme(theme) {
    if (theme == "dark") {
        var hamburger = document.querySelector(".navbar-toggler");
        hamburger.classList.add("navbar-dark");
        hamburger.classList.remove("navbar-light");
    }
    else {
        var hamburger = document.querySelector(".navbar-toggler");
        hamburger.classList.add("navbar-light");
        hamburger.classList.remove("navbar-dark");
    }

    html.dataset.theme = `theme-${theme}`;
    setTheme(theme);
}
function setTheme(theme) {
    localStorage.setItem("theme", theme)
}

// for checking the theme var on load
function validateTheme(theme) {
    if (theme != null) {
        if (theme === 'dark') {
            switchTheme('dark')
            document.querySelector("#btnradio3").setAttribute('checked', "true")
        }

        else {
            switchTheme('light')
            document.querySelector("#btnradio1").setAttribute('checked', "true")
        }
    }
    else {
        switchTheme("light")
        setTheme("light")
        document.querySelector("#btnradio1").setAttribute('checked', "true")
    }
}

// when the key theme is change this event fires  to swap the theme
window.addEventListener('storage', function (event) {
    if (event.key === 'theme') {
        var newValue = event.newValue;
        switchTheme(newValue);
    }

});

// used to stage a success message on new page load. 
function triggerSuccess() {
    $("#success-modal").modal('show')
    localStorage.setItem('result', '')
}
// used to stage a failure message on new page load. 
function triggerFailure() {
    $("#fail-modal").modal('show')
    localStorage.setItem('result', '')
}


// When the window load this fires
// to add the loaded class to the body which initiates the fade in effect
window.onload = function () {
    document.body.classList.add('loaded');
    // to trigger the success message if a success value is in the result key. Pops up a modal with the word success.
    if (localStorage.getItem("result") == "success") {
        triggerSuccess()
    }
    // Same as above but for a failure. Red text, nasty stuff.
    if (localStorage.getItem("result") == "fail") {
        triggerFailure();
    }
}


let logo = document.getElementById("logo")
// The onclick event that will initiate the animation of the logo. 
logo.addEventListener("click", async (e) => {
    if (logo.classList.contains('runawaycart')) {
        logo.classList.remove("runawaycart")
    }
    logo.classList.add("runawaycart");

})


ApplyIndexEventListeners()

let deleteShoppingList = document.querySelector('[data-open-modal-delete-shopping-list]');
let renameShoppingList = document.querySelector('[data-open-modal-rename-shopping-list]')


// This block triggers if the page contains the delete shopping list button.

//it is for oppening the modal for interacting with the delete function, applies the event listeners in the modal on the page. 
if (deleteShoppingList == null) {

}
else {

    // Rename button  and modal event listeners. 
    renameShoppingList.addEventListener("click", async (e) => {
        showSpinner()
        let response = await fetch(`/ShoppingLists/EditName/${e.target.id}`)
        let htmlResponse = await response.text();
        hideSpinner()
        document.getElementById("shopping-list-modal-body").innerHTML = htmlResponse;
        $("#shopping-list-modal").modal('show')

        let cancel = document.querySelector("#close-shopping-list-modal");
        let submit = document.querySelector(".button-submit");

        submit.addEventListener("click", async () => {
            showSpinner()
            let input = document.querySelector("#rename-input").value.trim();

            if (input == document.querySelector("#listName").value) {
                $("#rename-validation").text("List names can not be the same.");
                hideSpinner()
                return;
            }
            if (input.length < 4) {
                $("#rename-validation").text("Minimum of 4 characters")
            }
            else {
                let id = submit.id;
                let response = await fetch(`/ShoppingLists/EditName/?id=${id}&name=${input}`, {
                    method: 'PUT',
                    headers: {
                        'content-type': 'application/json'
                    }

                });
                await StoreResult(response)
                if (await response.ok) {
                    window.location.href = await response.url;
                }
                else {
                    hideSpinner()
                    $("#rename-validation").text("Names can not be duplicate.");
                    location.reload();
                }
                ApplyIndexEventListeners();
            }

            hideSpinner()
        })

        cancel.addEventListener("click", async (e) => {
            $("#shopping-list-modal").modal('hide')
        })

    })

    // delete button and modal event listeners. 
    deleteShoppingList.addEventListener("click", async (e) => {
        showSpinner()
        let response = await fetch(`/ShoppingLists/Delete/${e.target.id}`, {
            method: 'GET',
            headers: {
                'content-type': 'application/json'
            }

        });
        let htmlResponse = await response.text();
        document.getElementById("shopping-list-modal-body").innerHTML = htmlResponse;
        hideSpinner()
        $("#shopping-list-modal").modal('show')

        let cancel = document.getElementById("close-shopping-list-modal");

        cancel.addEventListener("click", async (e) => {
            $("#shopping-list-modal").modal('hide')
        })

        let submit = document.querySelector(".button-submit");

        submit.addEventListener("click", async () => {
            let id = submit.id;
            let response = await fetch(`/ShoppingLists/Delete/?id=${id}`, {
                method: 'DELETE',
                headers: {
                    'content-type': 'application/json'
                }

            });
            await StoreResult(response)
            window.location.href = await response.url;
        })

    })

}


// fires if on the edit shopping list page
// For applying the delete list item/product from the shopping list.
// opens a dialog applies the button even listers. 

let deleteProduct = document.getElementsByClassName("delete-product");
let deleteDialogs = document.getElementsByClassName("dialog-delete");

if (deleteProduct == null) {

} else {
    let dialogCount = deleteDialogs.length;
    for (let x = 0; x < dialogCount; x++) {

        deleteProduct[x].addEventListener("click", async (e) => {
            let detailsId = e.target.id;

            closeAndClearAllDialogs();

            let deleteResponse = await fetch(`/ListDetails/Delete/?listDetailsId=${detailsId}`);
            let deleteHtmlResponse = await deleteResponse.text();
            deleteDialogs[x].innerHTML = deleteHtmlResponse;
            deleteDialogs[x].show();

            applyCancelDialogEventListener()

        })
    }
}


// fires if on the edit shopping list page
// For applying the edit list item/product on the shopping list.
// opens a dialog applies the button event listers. 
let editProduct = document.getElementsByClassName("edit-quantity");
let editDialogs = document.getElementsByClassName("dialog-edit");

if (editProduct == null) {

}
else {
    ApplyEditPageEventListeners();
}

// checks the response of a fetch.
// if the status code returned is 200 store success in the result key of the local storage.
// other wise store fail 
// the value is read at the start of each page read and will open a modal/message to let the user know the result of the action take. 
async function StoreResult(response) {
    if (await response.status == 200) {
        storeSuccess()
    }
    else {
        storeFaiure()
    }
}

// apply the Event lister for the add product button.
function ApplyOpenProductModalEventListeners() {
    let AddProductButton = document.querySelector("[data-open-modal-add]");

    if (AddProductButton == null) {
    }
    else {
        AddProductButton.addEventListener("click", async (e) => {
            await PopulateProductModal(e);
        });
    }
}

// runs the Edit page event listers applies the edit dialog to each edit button. 
function ApplyEditPageEventListeners() {
    ApplyOpenProductModalEventListeners();
    let dialogCount = editDialogs.length;
    for (let k = 0; k < dialogCount; k++) {

        editProduct[k].addEventListener("click", async (e) => {
            showSpinner()
            let detailsId = e.target.id;
            let existingQuantity = parseInt($(`#${detailsId}-quantity`).text().trim());


            closeAndClearAllDialogs();

            let listId = document.querySelector("#listId").value;
            let editResponse = await fetch(`/ListDetails/Edit/?productId=0&listId=${listId}&listDetailsId=${detailsId}`);
            let editHtmlResponse = await editResponse.text();

            editDialogs[k].innerHTML = editHtmlResponse;
            editDialogs[k].show();
            hideSpinner();
            let quantity = existingQuantity;
            let quantityDisplay = document.getElementById("quantity-display");
            let quantityHidden = document.getElementById("quantity-submit");
            quantityDisplay.textContent = quantity.toString();
            quantityHidden.value = quantity;
            let quantityUp = document.getElementById("quantity-increase");
            let quantityDown = document.getElementById("quantity-decrease");

            quantityUp.addEventListener("click", async (e) => {
                if (quantity < 99) {
                    quantity++;
                    quantityDisplay.textContent = quantity.toString();
                    quantityHidden.value = quantity;
                }

            });

            quantityDown.addEventListener("click", async (e) => {
                if (quantity > 0) {
                    quantity--;
                    quantityDisplay.textContent = quantity.toString();
                    quantityHidden.value = quantity;
                }

            });

            let savebutton = document.querySelector("#save-quantity");
            savebutton.addEventListener("click", (e) => {
                if (quantity < 99 && quantity > 0) {
                    storeSuccess();
                }

            })


            applyCancelDialogEventListener();

        });
    }
}

// get the data from the server to display the product modal's HTML. 
// applies the event listeners to the modal elements. 
async function PopulateProductModal(e) {
    showSpinner()
    let response = await fetch(`/ListDetails/Create/?id=${e.target.id}`);
    let htmlResponse = await response.text();
    document.getElementById('addProductModalBody').innerHTML = htmlResponse;
    $("#addProductModal").modal('show');
    let closeProductListButton = document.querySelector("#closeProductModal");

    closeProductListButton.addEventListener("click", () => {
        $('#addProductModal').modal("hide");
    });

    let searchProductButton = document.querySelector(".search-products-button");

    if (searchProductButton == null) {

    }
    else {
        searchProductButton.addEventListener("click", async (e) => {
            let searchText = document.getElementById('product-search').value.trim();
            if (searchText.length < 4) {
                $("#search-validation").text("minimum 4 characters")
            }
            else {
                await updateProductList(searchText);
                productModalScript();
            }

        });
    }

    await updateProductList();
    productModalScript();
    hideSpinner()
}

async function updateProductList(search = '') {
    let listId = document.querySelector(".search-products-button");

    let response = await fetch(`/Products/GetProductList/?id=${listId.id}&search=${search}`);
    let htmlResponse = await response.text();

    document.querySelector('#product-list-partial').innerHTML = htmlResponse;
}

async function ApplyIndexEventListeners() {

    var failmodal = document.querySelector("#name-fail-modal")
    if (failmodal == null) {

    } else {
        $('#name-fail-modal').modal('show');
    }

    let updateListDetails = document.getElementsByClassName("shopping-list")
    let length = updateListDetails.length
    for (let i = 0; i < length; i++) {
        updateListDetails[i].addEventListener("click", async (e) => {
            await PopulateListDetails(e);
        });
    }


    // Set up the Create list interface Event listener. 
    //     set up the submit/save button event listener for posting data to the endpoint. 
    let openCreateListButton = document.querySelector("[data-open-modal-create]")
    if (openCreateListButton == null) {

    }
    else {
        openCreateListButton.addEventListener("click", async () => {
            showSpinner()
            let response = await fetch('/ShoppingLists/Create');
            let htmlResponse = await response.text();
            document.getElementById('createModalBody').innerHTML = htmlResponse
            hideSpinner()
            $('#createModal').modal("show");

            let newListSubmit = document.querySelector('#new-list-submit')

            newListSubmit.addEventListener('submit', async (e) => {
                await handleCreateList(e);
            })

            let closeCreateListButton = document.querySelector("#closeCreateModal")

            closeCreateListButton.addEventListener("click", () => {
                $('#createModal').modal("hide");
            })
        })
    }


}

// manages the validation of the create list modal. 
// Sends create list fetch request. 
async function handleCreateList(e) {
    e.preventDefault();
    let input = document.querySelector("#new-list-input").value.trim();
    let shoppingLists = document.querySelectorAll(".list-name")
    let shoppingListsLength = shoppingLists.length;


    if (input.length < 4) {
        $("#new-list-validation").text("List names are required to be a minimum of 4 characters.");
    }
    else {
        for (var i = 0; i < shoppingListsLength; i++) {
            if (input == shoppingLists[i].innerText) {
                $("#new-list-validation").text("Lists must be unique");
                return;
            }
        }
        let response = await fetch(`/ShoppingLists/Create`, {
            method: 'POST',
            headers: {
                'content-type': 'application/json'
            },
            body: JSON.stringify(e.target["Name"].value)
        });


        window.location.href = await response.url;
        ////document.querySelector('body').innerHTML = await response.text();
        //ApplyEditPageEventListeners();
        //ApplyIndexEventListeners();


    }
}

//populates the list details into the shopping list index page as a partial view.
async function PopulateListDetails(e) {

    let response = await fetch(`/ShoppingLists/Details/${e.target.id}`);
    let htmlResponse = await response.text();
    document.getElementById('list-details-partial').innerHTML = htmlResponse;

    let openProductListButton = document.querySelector("[data-open-modal-add]");

    if (openProductListButton == null) {
    }
    else {
        openProductListButton.addEventListener("click", async (e) => {

            await PopulateProductModal(e);
        });
    }

}

//closes and destroys all dialogs when closing dialogs.
function closeAndClearAllDialogs() {
    let allDialogs = document.getElementsByTagName("dialog")
    let allDialogsLength = allDialogs.length
    for (let j = 0; j < allDialogsLength; j++) {
        allDialogs[j].close();
        allDialogs[j].innerHTML = "";
    }
}

// applies event listeners to the clost buttons on modals.
function applyCancelDialogEventListener() {
    let closeDialogButton = document.getElementById("close-dialog")

    closeDialogButton.addEventListener('click', async (e) => {
        closeAndClearAllDialogs()
    })
}
// the script which sets up populates the product modal with data, apply event listeners and all sub moduals.

// Validation of the edit quantity modal.  
async function productModalScript() {


    let descriptions = document.querySelectorAll(".description");
    let togglebuttons = document.getElementsByClassName("toggle");
    let descrptionsLength = descriptions.length;
    if (descriptions.length < 1) {
    }
    else {
        for (let i = 0; i < descrptionsLength; i++) {
            applyShowMoreEventListener(togglebuttons, i, descriptions);
        }
    }

    let addProduct = document.getElementsByClassName("add-product-button");
    let createDialogs = document.getElementsByClassName("dialog-create")

    if (addProduct == null) {

    }
    else {
        let productLength = addProduct.length;
        for (let i = 0; i < productLength; i++) {

            addProduct[i].addEventListener("click", async (e) => {
                let productId = e.target.id

                closeAndClearAllDialogs()

                let listId = document.querySelector("#listId").value;

                let response = await fetch(`/ListDetails/Edit/?productId=${productId}&listId=${listId}&listDetailsId=0`);
                let htmlResponse = await response.text();

                createDialogs[i].innerHTML = htmlResponse;
                createDialogs[i].show();

                let quantity = 1;
                let quantityDisplay = document.getElementById("quantity-display");
                let quantityHidden = document.getElementById("quantity-submit");
                quantityDisplay.textContent = quantity.toString();
                quantityHidden.value = quantity;
                let quantityUp = document.getElementById("quantity-increase");
                let quantityDown = document.getElementById("quantity-decrease");

                quantityUp.addEventListener("click", async (e) => {
                    if (quantity < 99) {
                        quantity++;
                        quantityDisplay.textContent = quantity.toString();
                        quantityHidden.value = quantity;
                    }

                })

                quantityDown.addEventListener("click", async (e) => {
                    if (quantity > 1) {
                        quantity--;
                        quantityDisplay.textContent = quantity.toString();
                        quantityHidden.value = quantity;
                    }

                })

                let savebutton = document.querySelector("#save-quantity");
                savebutton.addEventListener("click", (e) => {
                    if (quantity < 99 && quantity > 0) {
                        storeSuccess();
                    }

                })
                applyCancelDialogEventListener()
            })
        }
    }
}

function applyShowMoreEventListener(togglebuttons, i, descriptions) {
    togglebuttons[i].addEventListener("click", () => {
        descriptions[i].classList.toggle('show-description');
    });
}

function showSpinner() {
    $('#SpinnerModal').modal('show')
}
function hideSpinner() {
    $("#SpinnerModal").modal("hide")
}
