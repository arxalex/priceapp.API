Vue.component('itemsaver', {
    template: `
        <div class="row">
            <table class="table">
                <tbody>
                    <tr>
                        <td>
                            <span>Image: </span>
                        </td>
                        <td>
                            <img :src="sourceItem.image">
                        </td>
                        <td>
                            <img :src="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Image url: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.image }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.image">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Label: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.label }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.label">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Category: </span>
                        </td>
                        <td>
                            <span>{{ sourceLabels.categoryLabel }} ({{ sourceItem.category }})</span>
                        </td>
                        <td>
                            <select class="form-select" v-model="destinationItem.category">
                                <option disabled>Chose category</option>
                                <option v-for="category in categories" v-bind:value="category.id">{{ category.label }}</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Brand: </span>
                        </td>
                        <td>
                            <span>{{ sourceLabels.brandLabel }} ({{ sourceItem.brand }})</span>
                        </td>
                        <td>
                            <select class="form-select" v-model="destinationItem.brand">
                                <option disabled>Chose brand</option>
                                <option v-for="brand in brands" v-bind:value="brand.id">{{ brand.label }}</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Consist: </span>
                        </td>
                        <td>
                            <span v-for="consistLabel in sourceLabels.consistLabels">{{ consistLabel }}</span>
                        </td>
                        <td>
                            <select class="form-select" multiple v-model="destinationItem.consist">
                                <option disabled>Chose consists</option>
                                <option v-for="consist in consists" v-bind:value="consist.id">{{ consist.label }}</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Units: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.units }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.units">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Term: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.term }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.term">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Calorie: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.calorie }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.calorie">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Carbohydrates: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.carbohydrates }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.carbohydrates">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Fat: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.fat }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.fat">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <span>Proteins: </span>
                        </td>
                        <td>
                            <span>{{ sourceItem.proteins }}</span>
                        </td>
                        <td>
                            <input v-model="destinationItem.proteins">
                        </td>
                    </tr>
                </tbody>
            </table>
            <button class="btn btn-primary" v-on:click='$emit("insert")'></button>
        </div>
    `,
    data() {
        return {
            sourceLabels: {
                categoryLabel: "",

            },
            categories: []
        }
    },
    methods: {
        change: function() {

        },
        get_categories: function() {

        }
    },
    props: {
        soureItem: {
            type: Object
        },
        destinationItem: {
            type: Object
        }
    }
});

